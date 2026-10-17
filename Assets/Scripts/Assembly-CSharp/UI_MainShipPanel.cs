public class UI_MainShipPanel : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform shipModelPositionTrans;

	private Flag shipFlag;

	private global::UnityEngine.GameObject modelObj;

	private global::UnityEngine.Transform modelTrans;

	private int uiLayer;

	private MainUpgradePrice upgradeMainPriceData;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text currentHpText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text healthText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text damageText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text cannonCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text coolTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text priceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image currentHpTop;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject healSpeechObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject hpBarObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform modelRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic[] BtnTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image btnImg;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image btnCoinImg;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup btnCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject oriDirectionalLightObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject upgradeDirectionalLightObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Light upgradeDirectionalLight;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem upgradePs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem upgradeParticleSystem;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform circleWhiteRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image circleWhiteImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject touchblockObj;

	private ShipBaseData myBaseData;

	private void Awake()
	{
		uiLayer = global::UnityEngine.LayerMask.NameToLayer("UI");
		upgradeMainPriceData = global::UnityEngine.Resources.Load<MainUpgradePrice>("Data/MainUpgradePrice");
	}

	private void OnEnable()
	{
		ModelRefresh();
		DataRefresh();
		HpCheckLayoutSet();
		TutorialManager.Instance.StartTutorial("upgradepopup");
		upgradeDirectionalLightObj.SetActive(value: true);
		oriDirectionalLightObj.SetActive(value: false);
	}

	public void OriginalLightBack()
	{
		upgradeDirectionalLightObj.SetActive(value: false);
		oriDirectionalLightObj.SetActive(value: true);
	}

	private void HpCheckLayoutSet()
	{
		if (GameManager.Instance.playerShip.myHealth.HpRatio >= 1f)
		{
			hpBarObj.SetActive(value: false);
			healSpeechObj.SetActive(value: false);
			leftTimeText.gameObject.SetActive(value: false);
			currentHpText.rectTransform.anchoredPosition3D = new global::UnityEngine.Vector3(18f, -99f, -3000f);
			modelRectTransform.anchoredPosition3D = new global::UnityEngine.Vector3(7f, 66f, -200f);
		}
		else
		{
			hpBarObj.SetActive(value: true);
			healSpeechObj.SetActive(value: true);
			leftTimeText.gameObject.SetActive(value: true);
			currentHpText.rectTransform.anchoredPosition3D = new global::UnityEngine.Vector3(18f, -186f, -3000f);
			modelRectTransform.anchoredPosition3D = new global::UnityEngine.Vector3(7f, -21f, -200f);
		}
	}

	public void MainUpgrade()
	{
		if (!isCanUpgrade())
		{
			return;
		}
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt mainShipLevel = PlayerInfo.Instance.myData.mainShipLevel;
		float hpRatio = GameManager.Instance.playerShip.myHealth.HpRatio;
		++PlayerInfo.Instance.myData.mainShipLevel;
		if ((bool)GameManager.Instance.playerShip)
		{
			int num = (int)((float)PlayerInfo.Instance.PlayerShipHpMax * hpRatio);
			PlayerInfo.Instance.myData.lastLeftHp = num;
			GameManager.Instance.playerShip.myHealth.SetHealthPoint(num, PlayerInfo.Instance.PlayerShipHpMax);
		}
		if (PlayerInfo.Instance.myData.selectedSkin == eShipSkin.Normal)
		{
			if ((int)PlayerInfo.Instance.myData.mainShipLevel % 25 == 0 && (int)PlayerInfo.Instance.myData.mainShipLevel != ConstValue.ShipLevelMax)
			{
				ShipBase playerShip = GameManager.Instance.playerShip;
				EShipMain level = (EShipMain)global::UnityEngine.Mathf.Min((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 10);
				ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
				int num2 = (int)PlayerInfo.Instance.myData.mainShipLevel % 100 / 25;
				global::UnityEngine.GameObject gameObject = null;
				switch (num2)
				{
				case 0:
					gameObject = mainData.modelSrc;
					break;
				case 1:
					gameObject = mainData.playerModelType01;
					break;
				case 2:
					gameObject = mainData.playerModelType02;
					break;
				case 3:
					gameObject = mainData.playerModelType03;
					break;
				}
				if ((int)PlayerInfo.Instance.myData.mainShipLevel >= ConstValue.ShipLevelMax)
				{
					gameObject = mainData.playerModelType03;
				}
				if (gameObject != null)
				{
					playerShip.SpawnShipModel(gameObject, ELayerType.Player);
				}
				else
				{
					playerShip.SpawnShipModel(mainData.modelSrc, ELayerType.Player);
				}
				if ((int)PlayerInfo.Instance.myData.mainShipLevel % 100 == 0)
				{
					StartCoroutine(EvolutionAction());
				}
				else
				{
					ModelRefresh();
				}
			}
			else
			{
				global::DG.Tweening.ShortcutExtensions.DOKill(modelTrans);
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(modelTrans, 1.15f, 0.07f), delegate
				{
					global::DG.Tweening.ShortcutExtensions.DOScale(modelTrans, 1f, 0.12f);
				});
				upgradePs.Play();
			}
		}
		else
		{
			if ((int)PlayerInfo.Instance.myData.mainShipLevel % 100 == 0)
			{
				GameManager.Instance.playerShipController.RefreshModel();
			}
			global::DG.Tweening.ShortcutExtensions.DOKill(modelTrans);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(modelTrans, 1.15f, 0.07f), delegate
			{
				global::DG.Tweening.ShortcutExtensions.DOScale(modelTrans, 1f, 0.12f);
			});
			upgradePs.Play();
		}
		mainShipLevel = global::UnityEngine.Mathf.Clamp((int)mainShipLevel - 1, 0, 999);
		long gold = upgradeMainPriceData.dataArray[(int)mainShipLevel].Gold;
		PlayerInfo.Instance.AddMoney(EMonenyType.gold, -gold, isSave: false);
		PlayerInfo.Instance.SaveData();
		DataRefresh();
		AudioManager.Instance.PlaySFX("ui_upgrade");
	}

	private global::System.Collections.IEnumerator EvolutionAction()
	{
		touchblockObj.SetActive(value: true);
		upgradeParticleSystem.Play();
		global::DG.Tweening.ShortcutExtensions.DOIntensity(upgradeDirectionalLight, 60f, 2.5f);
		yield return new global::UnityEngine.WaitForSeconds(2.5f);
		circleWhiteRect.gameObject.SetActive(value: true);
		circleWhiteImage.color = global::UnityEngine.Color.white;
		circleWhiteRect.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(circleWhiteRect, 1f, 0.13f), global::DG.Tweening.Ease.InCirc), delegate
		{
			ModelRefresh();
		});
		yield return new global::UnityEngine.WaitForSeconds(1f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOColor(circleWhiteImage, new global::UnityEngine.Color(1f, 1f, 1f, 0f), 2f), delegate
		{
			circleWhiteRect.gameObject.SetActive(value: false);
		});
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOIntensity(upgradeDirectionalLight, 1.2f, 2f), delegate
		{
			touchblockObj.SetActive(value: false);
		});
	}

	private bool isCanUpgrade()
	{
		if ((int)PlayerInfo.Instance.myData.mainShipLevel == ConstValue.ShipLevelMax)
		{
			return false;
		}
		bool result = true;
		int num = PlayerInfo.Instance.myData.mainShipLevel;
		num = global::UnityEngine.Mathf.Clamp(num - 1, 0, 999);
		long gold = upgradeMainPriceData.dataArray[num].Gold;
		if ((long)PlayerInfo.Instance.myData.gold < gold)
		{
			result = false;
		}
		return result;
	}

	private void ModelRefresh()
	{
		if (modelObj != null)
		{
			global::UnityEngine.Object.Destroy(modelObj);
		}
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num = (int)PlayerInfo.Instance.myData.mainShipLevel % 100 / 25;
		global::UnityEngine.GameObject gameObject = null;
		if (PlayerInfo.Instance.myData.selectedSkin == eShipSkin.Normal)
		{
			switch (num)
			{
			case 0:
				gameObject = mainData.modelSrc;
				break;
			case 1:
				gameObject = mainData.playerModelType01;
				break;
			case 2:
				gameObject = mainData.playerModelType02;
				break;
			case 3:
				gameObject = mainData.playerModelType03;
				break;
			}
			if ((int)PlayerInfo.Instance.myData.mainShipLevel >= ConstValue.ShipLevelMax)
			{
				gameObject = mainData.playerModelType03;
			}
			if (gameObject != null)
			{
				modelObj = global::UnityEngine.Object.Instantiate(gameObject, shipModelPositionTrans);
			}
			else
			{
				modelObj = global::UnityEngine.Object.Instantiate(mainData.modelSrc, shipModelPositionTrans);
			}
		}
		else
		{
			ShipSkinData skinData = SkinDataManager.Instance.GetSkinData(PlayerInfo.Instance.myData.selectedSkin);
			modelObj = global::UnityEngine.Object.Instantiate(skinData.modelSrc, shipModelPositionTrans);
		}
		modelTrans = modelObj.transform;
		modelTrans.localScale = global::UnityEngine.Vector3.one;
		modelTrans.localRotation = global::UnityEngine.Quaternion.identity;
		modelTrans.localPosition = global::UnityEngine.Vector3.zero;
		global::UnityEngine.Transform[] componentsInChildren = modelObj.GetComponentsInChildren<global::UnityEngine.Transform>();
		shipFlag = modelObj.GetComponentInChildren<Flag>();
		global::UnityEngine.Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = uiLayer;
		}
		if (FlagManager.Instance.GetFlagInfo(PlayerInfo.Instance.myData.selectedFlagId, out var info))
		{
			shipFlag.SetFlag(info.flagMaterial);
		}
		GameManager.Instance.playerShipController.GenerateCannonRange();
	}

	private void DataRefresh()
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt mainShipLevel = PlayerInfo.Instance.myData.mainShipLevel;
		myBaseData = ShipDataManager.Instance.GetShipBaseData(EShipType.Main);
		if ((int)PlayerInfo.Instance.myData.mainShipLevel == ConstValue.ShipLevelMax)
		{
			priceText.gameObject.SetActive(value: false);
			btnCoinImg.gameObject.SetActive(value: false);
			BtnTexts[0].rectTransform.anchoredPosition = global::UnityEngine.Vector2.zero;
			BtnTexts[0].GetComponent<global::UnityEngine.UI.Text>().text = "MAX";
			levelText.text = "Lv.MAX";
		}
		else
		{
			levelText.text = $"Lv.{mainShipLevel}";
		}
		int level = global::UnityEngine.Mathf.Clamp((int)mainShipLevel / 100 + 1, 1, 10);
		ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(myBaseData.id, (EShipSub)level);
		if (subShipData == null || subShipData.modelSrc == null)
		{
			subShipData = ShipDataManager.Instance.GetSubShipData(myBaseData.id, EShipSub.LEVEL_01);
		}
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(myBaseData.id, mainShipLevel);
		healthText.text = Numbering.Convert(shipSheetData.health);
		damageText.text = Numbering.Convert(shipSheetData.attack);
		cannonCountText.text = "x" + ShipDataManager.Instance.GetMainData((EShipMain)level).cannonCount;
		mainShipLevel = global::UnityEngine.Mathf.Clamp((int)mainShipLevel - 1, 0, 999);
		coolTimeText.text = string.Format("{0}{1}", ShipDataManager.Instance.mainShipData.dataArray[(int)mainShipLevel].Firecooltime.ToString("F1"), LanguageManager.Instance.translate("key_sec"));
		long gold = upgradeMainPriceData.dataArray[(int)mainShipLevel].Gold;
		priceText.text = Numbering.Convert(gold);
		if (isCanUpgrade())
		{
			btnCanvasGroup.alpha = 1f;
		}
		else
		{
			btnCanvasGroup.alpha = 0.4f;
		}
	}

	private void LateUpdate()
	{
		CurrentHpRefresh();
	}

	private void CurrentHpRefresh()
	{
		int hP = GameManager.Instance.playerShip.myHealth.HP;
		int hP_MAX = GameManager.Instance.playerShip.myHealth.HP_MAX;
		float hpRatio = GameManager.Instance.playerShip.myHealth.HpRatio;
		currentHpTop.fillAmount = hpRatio;
		currentHpText.text = $"{Numbering.Convert(hP)} / {Numbering.Convert(hP_MAX)}";
		float num = 0f;
		float num2 = ConstValue.HealthRegenerationTime;
		num = num2 * hpRatio;
		float num3 = num2 - num;
		leftTimeText.text = $"{(int)(num3 / 60f):00} : {num3 % 60f:00}";
		HpCheckLayoutSet();
	}
}
