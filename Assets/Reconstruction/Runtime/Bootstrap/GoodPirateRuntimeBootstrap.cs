using System;
using System.IO;
using System.Runtime.InteropServices;
using GoodPirate113.Reconstruction.Core;
using GoodPirate113.Reconstruction.Input;
using GoodPirate113.Reconstruction.Modal;
using GoodPirate113.Reconstruction.Rendering;
using GoodPirate113.Reconstruction.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodPirate113.Reconstruction.Bootstrap
{
    public enum GoodPirateStorageBackend
    {
        Auto,
        File,
        WebGlLocalStorage
    }

    [DefaultExecutionOrder(-10000)]
    public sealed class GoodPirateRuntimeBootstrap : MonoBehaviour
    {
        public const int MaxTransferCharacters = 256 * 1024;

        [SerializeField] private GoodPirateStorageBackend storageBackend = GoodPirateStorageBackend.Auto;
        [SerializeField] private bool persistAcrossScenes = true;
        [SerializeField] private bool saveOnPause = true;

        private GoodPirate113SaveRepository saveRepository;
        private bool initialized;
        private float loadingSceneEnteredAt;
        private bool startupFallbackAttempted;

        public static GoodPirateRuntimeBootstrap Instance { get; private set; }

        public ISaveStore SaveStore { get; private set; }
        public IUtcClock Clock { get; private set; }
        public IRandomSource Random { get; private set; }
        public IGoodPirateInputSource InputSource { get; private set; }
        public IModalCoordinator Modals { get; private set; }
        public GoodPirate113SaveV1 CurrentSave { get; private set; }
        public string LastLoadWarning { get; private set; } = string.Empty;
        public bool LoadedExistingSave { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GoodPirate113PointerInstall(string receiver);
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureRuntimeExists()
        {
            if (Instance != null || FindFirstObjectByType<GoodPirateRuntimeBootstrap>() != null)
            {
                return;
            }

            var runtime = new GameObject("GoodPirateRuntimeBootstrap");
            runtime.AddComponent<GoodPirateRuntimeBootstrap>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializeDefaults();
            SceneManager.sceneLoaded += HandleSceneLoaded;
            HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            if (Modals != null)
            {
                Modals.ModalStateChanged -= HandleModalStateChanged;
            }

            SceneManager.sceneLoaded -= HandleSceneLoaded;

            Instance = null;
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && saveOnPause)
            {
                TrySaveWithLogging();
            }
        }

        private void OnApplicationQuit()
        {
            if (saveOnPause)
            {
                TrySaveWithLogging();
            }
        }

        private void Update()
        {
            if (startupFallbackAttempted || SceneManager.GetActiveScene().name != "Loading")
            {
                return;
            }

            if (Time.realtimeSinceStartup - loadingSceneEnteredAt < 2f)
            {
                return;
            }

            startupFallbackAttempted = true;
            Debug.LogWarning("GP113_STARTUP loading-timeout forcing-intro", this);
            if (Application.CanStreamedLevelBeLoaded("Intro"))
            {
                SceneManager.LoadScene("Intro");
            }
        }

        public void InitializeDefaults()
        {
            if (initialized)
            {
                return;
            }

            Clock = new SystemUtcClock();
            Random = new SystemRandomSource();
            SaveStore = CreateSaveStore();
            Modals = new ModalCoordinator();
            InputSource = GetComponent<LegacyUnityInputSource>();
            if (InputSource == null)
            {
                InputSource = gameObject.AddComponent<LegacyUnityInputSource>();
            }
            if (GetComponent<GoodPirateSteeringVisualRuntime>() == null)
            {
                gameObject.AddComponent<GoodPirateSteeringVisualRuntime>();
            }

            saveRepository = new GoodPirate113SaveRepository(SaveStore, Clock);
            CurrentSave = saveRepository.LoadOrCreate(out var loadedExisting, out var warning);
            LoadedExistingSave = loadedExisting;
            LastLoadWarning = warning;

            Modals.ModalStateChanged += HandleModalStateChanged;
            InputSource.IsEnabled = !Modals.IsModalOpen;
            initialized = true;

#if UNITY_WEBGL && !UNITY_EDITOR
            GoodPirate113PointerInstall(gameObject.name);
#endif

            if (!string.IsNullOrEmpty(LastLoadWarning))
            {
                Debug.LogWarning(LastLoadWarning, this);
            }
        }

        public void SaveNow()
        {
            EnsureInitialized();
            saveRepository.Save(CurrentSave);
        }

        public string ExportSaveDocument()
        {
            EnsureInitialized();
            var document = CurrentSave.Serialize();
            if (document.Length > MaxTransferCharacters)
            {
                throw new InvalidOperationException("The save is too large to transfer through the clipboard.");
            }
            return document;
        }

        public bool TryImportSaveDocument(string document, out string error)
        {
            EnsureInitialized();
            if (string.IsNullOrWhiteSpace(document))
            {
                error = "The clipboard does not contain a save.";
                return false;
            }
            if (document.Length > MaxTransferCharacters)
            {
                error = "The clipboard save is too large.";
                return false;
            }
            if (!GoodPirate113SaveV1.TryDeserialize(document.Trim(), out var imported, out error))
            {
                return false;
            }
            if (!GoodPirate113ProgressData.TryDeserialize(
                    imported.PayloadJson,
                    out var progress,
                    out error)
                || !progress.TryValidateForTransfer(out error))
            {
                return false;
            }

            progress.PrepareForImport();
            imported.PayloadJson = progress.Serialize();

            try
            {
                var normalized = imported.Serialize();
                if (SaveStore.TryLoad(GoodPirate113SaveV1.DefaultSlot, out var currentDocument)
                    && GoodPirate113SaveV1.TryDeserialize(currentDocument, out _, out _))
                {
                    SaveStore.Save(GoodPirate113SaveV1.ImportRecoverySlot, currentDocument);
                }
                SaveStore.Save(GoodPirate113SaveV1.DefaultSlot, normalized);
                CurrentSave = imported;
                LoadedExistingSave = true;
                LastLoadWarning = string.Empty;
                error = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                error = "The clipboard save could not be stored: " + exception.Message;
                return false;
            }
        }

        public bool TryRollbackImportedSave(out string error)
        {
            EnsureInitialized();
            error = string.Empty;
            try
            {
                if (!SaveStore.TryLoad(GoodPirate113SaveV1.ImportRecoverySlot, out var document)
                    || !GoodPirate113SaveV1.TryDeserialize(document, out var recovered, out error))
                {
                    error = string.IsNullOrEmpty(error)
                        ? "No valid pre-import save is available."
                        : error;
                    return false;
                }
                SaveStore.Save(GoodPirate113SaveV1.DefaultSlot, document);
                CurrentSave = recovered;
                LoadedExistingSave = true;
                LastLoadWarning = "The previous local save was restored after an import failure.";
                error = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                error = "The previous local save could not be restored: " + exception.Message;
                return false;
            }
        }

        public void ResetToFreshSave()
        {
            EnsureInitialized();
            saveRepository.Delete();
            SaveStore.Delete(GoodPirate113SaveV1.ImportRecoverySlot);
            CurrentSave = GoodPirate113SaveV1.Create(Clock);
            LoadedExistingSave = false;
            LastLoadWarning = string.Empty;
        }

        public void MarkStartupRouteRequested(string sceneName)
        {
            startupFallbackAttempted = true;
            Debug.Log("GP113_STARTUP route-requested " + sceneName, this);
        }

        public void OnBrowserPointerDown(string payload)
        {
            ForwardBrowserInput(nameof(OnBrowserPointerDown), payload);
        }

        public void OnBrowserPointerMove(string payload)
        {
            ForwardBrowserInput(nameof(OnBrowserPointerMove), payload);
        }

        public void OnBrowserPointerUp(string payload)
        {
            ForwardBrowserInput(nameof(OnBrowserPointerUp), payload);
        }

        public void OnBrowserPointerCancel(string payload)
        {
            ForwardBrowserInput(nameof(OnBrowserPointerCancel), payload);
        }

        public void OnBrowserInputLostFocus(string reason)
        {
            ForwardBrowserInput(nameof(OnBrowserInputLostFocus), reason);
            InputSource?.ReleaseAll();
        }

        private ISaveStore CreateSaveStore()
        {
            var fileStore = new FileSaveStore(
                Path.Combine(Application.persistentDataPath, "GoodPirate113"));

            switch (storageBackend)
            {
                case GoodPirateStorageBackend.File:
                    return fileStore;
                case GoodPirateStorageBackend.WebGlLocalStorage:
                    return new WebGlLocalStorageSaveStore(fileStore);
                case GoodPirateStorageBackend.Auto:
#if UNITY_WEBGL && !UNITY_EDITOR
                    return new WebGlLocalStorageSaveStore();
#else
                    return fileStore;
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleModalStateChanged(bool isModalOpen)
        {
            if (InputSource != null)
            {
                InputSource.IsEnabled = !isModalOpen;
            }
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("GP113_STARTUP scene-loaded " + scene.name, this);
            if (scene.name == "Loading")
            {
                loadingSceneEnteredAt = Time.realtimeSinceStartup;
                startupFallbackAttempted = false;
            }
        }

        private static void ForwardBrowserInput(string method, string payload)
        {
            var touchPanel = GameObject.Find("TouchPanel");
            if (touchPanel == null || !touchPanel.activeInHierarchy)
            {
                if (method == nameof(OnBrowserPointerDown))
                {
                    Debug.LogWarning("GP113_INPUT pointer-down missing-touch-panel");
                }
                return;
            }

            if (method == nameof(OnBrowserPointerDown))
            {
                Debug.Log("GP113_INPUT pointer-down forwarded");
            }
            touchPanel.SendMessage(method, payload, SendMessageOptions.DontRequireReceiver);
        }

        private void TrySaveWithLogging()
        {
            if (!initialized || CurrentSave == null)
            {
                return;
            }

            try
            {
                SaveNow();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Good Pirate 1.13 save failed: {exception.Message}", this);
            }
        }

        private void EnsureInitialized()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("The runtime bootstrap is not initialized.");
            }
        }
    }
}
