using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Platform
{
    public static class GoodPirateClipboardRuntime
    {
        public static void Copy(string value, Action succeeded, Action<string> failed)
        {
            GoodPirateClipboardBridge.Instance.Copy(value, succeeded, failed);
        }

        public static void Paste(Action<string> succeeded, Action<string> failed)
        {
            GoodPirateClipboardBridge.Instance.Paste(succeeded, failed);
        }

        public static void ShowManualCopy(string value)
        {
            GoodPirateClipboardBridge.Instance.ShowManualCopy(value);
        }

        public static void RequestManualPaste(Action<string> completed)
        {
            GoodPirateClipboardBridge.Instance.RequestManualPaste(completed);
        }

        public static void RequestImportConfirmation(string value, Action<string> confirmed)
        {
            GoodPirateClipboardBridge.Instance.RequestImportConfirmation(value, confirmed);
        }

        public static void ReportImportResult(bool succeeded, string message)
        {
            GoodPirateClipboardBridge.Instance.ReportImportResult(succeeded, message);
        }

		public static void RearmManualPaste(Action<string> completed)
		{
			GoodPirateClipboardBridge.Instance.RearmManualPaste(completed);
		}
    }

    public sealed class GoodPirateClipboardBridge : MonoBehaviour
    {
        private const string ObjectName = "Good Pirate Clipboard";
        private static GoodPirateClipboardBridge instance;
        private Action copySucceeded;
        private Action<string> copyFailed;
        private Action<string> pasteSucceeded;
        private Action<string> pasteFailed;
        private Action<string> manualPasteCompleted;
        private string pendingNativeImport = string.Empty;
        private bool showNativeImportConfirmation;

        public static GoodPirateClipboardBridge Instance
        {
            get
            {
                EnsureInstance();
                return instance;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInstance()
        {
            if (instance != null)
            {
                return;
            }

            var bridgeObject = new GameObject(ObjectName);
            DontDestroyOnLoad(bridgeObject);
            instance = bridgeObject.AddComponent<GoodPirateClipboardBridge>();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            ClearCopyCallbacks();
            ClearPasteCallbacks();
            manualPasteCompleted = null;
            pendingNativeImport = string.Empty;
            showNativeImportConfirmation = false;
        }

        internal void Copy(string value, Action succeeded, Action<string> failed)
        {
            copySucceeded = succeeded;
            copyFailed = failed;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (GoodPirate113ClipboardWrite(value, ObjectName) == 0)
            {
                CompleteCopyFailure("Clipboard API is unavailable.");
            }
#else
            try
            {
                GUIUtility.systemCopyBuffer = value;
                OnClipboardWrite("ok");
            }
            catch (Exception exception)
            {
                CompleteCopyFailure(exception.Message);
            }
#endif
        }

        internal void Paste(Action<string> succeeded, Action<string> failed)
        {
            pasteSucceeded = succeeded;
            pasteFailed = failed;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (GoodPirate113ClipboardRead(ObjectName) == 0)
            {
                CompletePasteFailure("Clipboard API is unavailable.");
            }
#else
            try
            {
                OnClipboardRead(GUIUtility.systemCopyBuffer);
            }
            catch (Exception exception)
            {
                CompletePasteFailure(exception.Message);
            }
#endif
        }

        internal void ShowManualCopy(string value)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GoodPirate113ClipboardManualCopy(value);
#else
            Debug.LogWarning("Manual clipboard copy is only available in WebGL.");
#endif
        }

        internal void RequestManualPaste(Action<string> completed)
        {
            manualPasteCompleted = completed;
#if UNITY_WEBGL && !UNITY_EDITOR
            GoodPirate113ClipboardManualPaste();
#else
            OnManualPaste(GUIUtility.systemCopyBuffer);
#endif
        }

        internal void RequestImportConfirmation(string value, Action<string> confirmed)
        {
			manualPasteCompleted = confirmed;
#if UNITY_WEBGL && !UNITY_EDITOR
			GoodPirate113ClipboardConfirmPaste(value);
#else
			pendingNativeImport = value ?? string.Empty;
			showNativeImportConfirmation = true;
#endif
		}

		internal void ReportImportResult(bool succeeded, string message)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			GoodPirate113ClipboardImportResult(succeeded ? 1 : 0, message ?? string.Empty);
#endif
		}

		internal void RearmManualPaste(Action<string> completed)
		{
			manualPasteCompleted = completed;
		}

        public void OnClipboardWrite(string result)
        {
            if (result == "ok")
            {
                var callback = copySucceeded;
                ClearCopyCallbacks();
                callback?.Invoke();
                return;
            }
            CompleteCopyFailure(result);
        }

        public void OnClipboardRead(string value)
        {
            var callback = pasteSucceeded;
            ClearPasteCallbacks();
            callback?.Invoke(value);
        }

        public void OnClipboardReadFailed(string reason)
        {
            CompletePasteFailure(reason);
        }

        public void OnManualPaste(string value)
        {
            var callback = manualPasteCompleted;
            manualPasteCompleted = null;
            pendingNativeImport = string.Empty;
            showNativeImportConfirmation = false;
            callback?.Invoke(value);
        }

		public void OnManualPasteCancelled(string reason)
		{
			manualPasteCompleted = null;
			pendingNativeImport = string.Empty;
			showNativeImportConfirmation = false;
		}

#if !UNITY_WEBGL || UNITY_EDITOR
        private void Update()
        {
            if (showNativeImportConfirmation && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                OnManualPasteCancelled("cancelled");
            }
        }

        private void OnGUI()
        {
            if (!showNativeImportConfirmation)
            {
                return;
            }

            GUI.depth = -10000;
            var fullScreen = new Rect(0f, 0f, Screen.width, Screen.height);
            GUI.Box(fullScreen, string.Empty);
            var width = Mathf.Min(620f, Screen.width - 40f);
            var height = 260f;
            var panel = new Rect(
                (Screen.width - width) * 0.5f,
                (Screen.height - height) * 0.5f,
                width,
                height);
            GUI.Box(panel, "PASTE SAVE");
            GUI.Label(
                new Rect(panel.x + 24f, panel.y + 54f, panel.width - 48f, 90f),
                "Replace local progress with this clipboard save?\nThe current save will remain as a backup.");
            if (GUI.Button(
                    new Rect(panel.x + 24f, panel.yMax - 74f, panel.width * 0.5f - 36f, 50f),
                    "Cancel"))
            {
                OnManualPasteCancelled("cancelled");
            }
            if (GUI.Button(
                    new Rect(panel.center.x + 12f, panel.yMax - 74f, panel.width * 0.5f - 36f, 50f),
                    "Replace Save"))
            {
                var value = pendingNativeImport;
                OnManualPaste(value);
            }
        }
#endif

        private void CompleteCopyFailure(string reason)
        {
            var callback = copyFailed;
            ClearCopyCallbacks();
            callback?.Invoke(reason);
        }

        private void CompletePasteFailure(string reason)
        {
            var callback = pasteFailed;
            ClearPasteCallbacks();
            callback?.Invoke(reason);
        }

        private void ClearCopyCallbacks()
        {
            copySucceeded = null;
            copyFailed = null;
        }

        private void ClearPasteCallbacks()
        {
            pasteSucceeded = null;
            pasteFailed = null;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GoodPirate113ClipboardWrite(string value, string receiver);

        [DllImport("__Internal")]
        private static extern int GoodPirate113ClipboardRead(string receiver);

        [DllImport("__Internal")]
        private static extern void GoodPirate113ClipboardManualCopy(string value);

        [DllImport("__Internal")]
        private static extern void GoodPirate113ClipboardManualPaste();

		[DllImport("__Internal")]
		private static extern void GoodPirate113ClipboardConfirmPaste(string value);

		[DllImport("__Internal")]
		private static extern void GoodPirate113ClipboardImportResult(int succeeded, string message);
#endif
    }
}
