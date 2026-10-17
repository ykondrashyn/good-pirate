public class UI_SettingPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnBgmOff;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnBgmOn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnSfxOff;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnSfxOn;

	private float prevTimescale;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject fps60TextObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject fps30TextObj;

	private void Awake()
	{
		EnsureSaveButtonLabel("btn_cloud up", "Copy\nSave");
		EnsureSaveButtonLabel("btn_cloud down", "Paste\nSave");
	}

	private void OnEnable()
	{
		prevTimescale = global::UnityEngine.Time.timeScale;
		global::UnityEngine.Time.timeScale = 0f;
		fps60TextObj.SetActive(PlayerInfo.Instance.myData.isTargetFps60);
		fps30TextObj.SetActive(!PlayerInfo.Instance.myData.isTargetFps60);
		if ((bool)PlayerInfo.Instance.myData.isMusicOn)
		{
			btnBgmOn.SetActive(value: false);
			btnBgmOff.SetActive(value: true);
		}
		else
		{
			btnBgmOn.SetActive(value: true);
			btnBgmOff.SetActive(value: false);
		}
		if ((bool)PlayerInfo.Instance.myData.isSoundOn)
		{
			btnSfxOn.SetActive(value: false);
			btnSfxOff.SetActive(value: true);
		}
		else
		{
			btnSfxOn.SetActive(value: true);
			btnSfxOff.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		global::UnityEngine.Time.timeScale = prevTimescale;
	}

	public void BgmOff()
	{
		btnBgmOff.SetActive(value: false);
		btnBgmOn.SetActive(value: true);
		PlayerInfo.Instance.MusicOff();
		BGMManager.Instance.StopBGM();
		AudioManager.PlayButtonClick();
	}

	public void BgmOn()
	{
		btnBgmOff.SetActive(value: true);
		btnBgmOn.SetActive(value: false);
		GameManager.Instance.MusicOn();
		PlayerInfo.Instance.MusicOn();
		AudioManager.PlayButtonClick();
	}

	public void SfxOff()
	{
		btnSfxOff.SetActive(value: false);
		btnSfxOn.SetActive(value: true);
		PlayerInfo.Instance.SoundOff();
		AudioManager.Instance.SFXStop();
	}

	public void SfxOn()
	{
		btnSfxOff.SetActive(value: true);
		btnSfxOn.SetActive(value: false);
		PlayerInfo.Instance.SoundOn();
		AudioManager.Instance.SFXStart();
		AudioManager.PlayButtonClick();
	}

	public void CloudUpload()
	{
		try
		{
			PlayerInfo.Instance.SaveData();
			if (MyInventoryDataManager.Instance != null)
			{
				MyInventoryDataManager.Instance.SaveData();
			}
			var preferences = GoodPirate113PortablePreferences.Capture();
			if (!global::GoodPirate113.Reconstruction.Save.GoodPirate113LegacyStateBridge.TrySavePreferences(preferences))
			{
				throw new global::System.InvalidOperationException("Portable progression could not be composed.");
			}
			var runtime = global::GoodPirate113.Reconstruction.Bootstrap.GoodPirateRuntimeBootstrap.Instance;
			if (runtime == null)
			{
				throw new global::System.InvalidOperationException("The save runtime is unavailable.");
			}
			string document = runtime.ExportSaveDocument();
			global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.Copy(
				document,
				delegate
				{
					ShowSaveTransferSuccess("Copy Save");
				},
				delegate(string reason)
				{
					global::UnityEngine.Debug.LogWarning("Clipboard copy fallback: " + reason);
					global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.ShowManualCopy(document);
				});
		}
		catch (global::System.Exception exception)
		{
			global::UnityEngine.Debug.LogError("Copy Save failed: " + exception.Message);
			ShowSaveTransferFailure();
		}
	}

	public void CloudDownload()
	{
		global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.Paste(
			RequestClipboardImportConfirmation,
			delegate(string reason)
			{
				global::UnityEngine.Debug.LogWarning("Clipboard paste fallback: " + reason);
				global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.RequestManualPaste(ImportClipboardSave);
			});
	}

	private void RequestClipboardImportConfirmation(string document)
	{
		global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.RequestImportConfirmation(
			document,
			ImportClipboardSave);
	}

	private void ImportClipboardSave(string document)
	{
		var runtime = global::GoodPirate113.Reconstruction.Bootstrap.GoodPirateRuntimeBootstrap.Instance;
		string error = "The save runtime is unavailable.";
		global::GoodPirate113.Reconstruction.Save.GoodPirate113SaveV1 inspectedSave;
		global::GoodPirate113.Reconstruction.Save.GoodPirate113ProgressData inspectedProgress;
		if (runtime == null
			|| !global::GoodPirate113.Reconstruction.Save.GoodPirate113SaveV1.TryDeserialize(
				document == null ? "" : document.Trim(),
				out inspectedSave,
				out error)
			|| !global::GoodPirate113.Reconstruction.Save.GoodPirate113ProgressData.TryDeserialize(
				inspectedSave.PayloadJson,
				out inspectedProgress,
				out error)
			|| !inspectedProgress.TryValidateForTransfer(out error)
			|| !GoodPirate113PortablePreferences.TryValidate(inspectedProgress.Preferences, out error)
			|| !runtime.TryImportSaveDocument(document, out error))
		{
			global::UnityEngine.Debug.LogWarning("Paste Save rejected: " + error);
			global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.RearmManualPaste(
				ImportClipboardSave);
			global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.ReportImportResult(
				false,
				"Save not imported. " + error);
			ShowSaveTransferFailure("Paste Save");
			return;
		}

		#if UNITY_WEBGL && !UNITY_EDITOR
		global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.ReportImportResult(
			true,
			"Save imported. Restarting the game...");
		#else
		try
		{
			if (!GoodPirate113PortablePreferences.TryApplyCurrent(out error))
			{
				throw new global::System.InvalidOperationException(error);
			}
			if (MyInventoryDataManager.Instance != null)
			{
				MyInventoryDataManager.Instance.ReloadFromReconstructionSave();
			}
			PlayerInfo.Instance.ReloadFromReconstructionSave();
			if (LanguageManager.Instance != null)
			{
				LanguageManager.Instance.ReloadFromPlayerInfo();
			}
			global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.ReportImportResult(
				true,
				"Save imported. Restarting the game...");
			ShowSaveTransferSuccess("Paste Save");
			global::UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
		}
		catch (global::System.Exception exception)
		{
			global::UnityEngine.Debug.LogError("Imported save could not be activated: " + exception.Message);
			if (runtime.TryRollbackImportedSave(out var rollbackError))
			{
				GoodPirate113PortablePreferences.TryApplyCurrent(out _);
			}
			else
			{
				global::UnityEngine.Debug.LogError(rollbackError);
			}
			global::GoodPirate113.Reconstruction.Platform.GoodPirateClipboardRuntime.ReportImportResult(
				false,
				"Save not imported. The previous local save was restored.");
			ShowSaveTransferFailure("Paste Save");
		}
		#endif
	}

	private void EnsureSaveButtonLabel(string buttonName, string labelText)
	{
		global::UnityEngine.Transform button = null;
		foreach (global::UnityEngine.Transform candidate in
			GetComponentsInChildren<global::UnityEngine.Transform>(includeInactive: true))
		{
			if (candidate.name == buttonName)
			{
				button = candidate;
				break;
			}
		}
		if (button == null || button.Find("Offline Save Label") != null)
		{
			return;
		}

		var labelObject = new global::UnityEngine.GameObject(
			"Offline Save Label",
			typeof(global::UnityEngine.RectTransform),
			typeof(global::UnityEngine.CanvasRenderer),
			typeof(global::UnityEngine.UI.Text),
			typeof(global::UnityEngine.UI.Outline));
		labelObject.transform.SetParent(button, worldPositionStays: false);
		var rect = labelObject.GetComponent<global::UnityEngine.RectTransform>();
		rect.anchorMin = new global::UnityEngine.Vector2(0f, 0f);
		rect.anchorMax = new global::UnityEngine.Vector2(1f, 0f);
		rect.pivot = new global::UnityEngine.Vector2(0.5f, 1f);
		rect.anchoredPosition = new global::UnityEngine.Vector2(0f, -5f);
		rect.sizeDelta = new global::UnityEngine.Vector2(40f, 72f);
		var label = labelObject.GetComponent<global::UnityEngine.UI.Text>();
		label.font = global::UnityEngine.Resources.GetBuiltinResource<global::UnityEngine.Font>("LegacyRuntime.ttf");
		label.fontSize = 25;
		label.fontStyle = global::UnityEngine.FontStyle.Bold;
		label.alignment = global::UnityEngine.TextAnchor.UpperCenter;
		label.color = global::UnityEngine.Color.white;
		label.raycastTarget = false;
		label.resizeTextForBestFit = true;
		label.resizeTextMinSize = 14;
		label.resizeTextMaxSize = 25;
		label.text = labelText;
		var outline = labelObject.GetComponent<global::UnityEngine.UI.Outline>();
		outline.effectColor = new global::UnityEngine.Color(0f, 0f, 0f, 0.8f);
		outline.effectDistance = new global::UnityEngine.Vector2(2f, -2f);
	}

	private static void ShowSaveTransferSuccess(string operation)
	{
		if (UI_Manager.Instance != null)
		{
			UI_Manager.Instance.CloudSuccessPopup(operation);
		}
	}

	private static void ShowSaveTransferFailure(string operation = "Save Transfer")
	{
		if (UI_Manager.Instance != null)
		{
			UI_Manager.Instance.CloudFailPopup(operation);
		}
	}

	public void OpenUrl(string url)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("http", "open-external-url");
	}

	public void OpenRateUs()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("http", "open-rate-page");
	}

	public void ToggleFps()
	{
		PlayerInfo.Instance.myData.isTargetFps60 = !PlayerInfo.Instance.myData.isTargetFps60;
		fps60TextObj.SetActive(PlayerInfo.Instance.myData.isTargetFps60);
		fps30TextObj.SetActive(!PlayerInfo.Instance.myData.isTargetFps60);
		if (PlayerInfo.Instance.myData.isTargetFps60)
		{
			global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(60);
		}
		else
		{
			global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(30);
		}
		PlayerInfo.Instance.SaveData();
	}
}
