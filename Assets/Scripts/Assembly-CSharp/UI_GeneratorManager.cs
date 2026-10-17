public class UI_GeneratorManager : global::UnityEngine.MonoBehaviour
{
	public static UI_GeneratorManager Instance;

	public global::System.Collections.Generic.Dictionary<EShipType, CoolTimeData> coolTimeData = new global::System.Collections.Generic.Dictionary<EShipType, CoolTimeData>();

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		foreach (global::System.Collections.Generic.KeyValuePair<EShipType, CoolTimeData> coolTimeDatum in coolTimeData)
		{
			if (coolTimeDatum.Value.isActive && coolTimeDatum.Value.LeftTimeRatio <= 0f)
			{
				if (coolTimeDatum.Value.unityAction != null)
				{
					coolTimeDatum.Value.unityAction();
				}
				coolTimeDatum.Value.isActive = false;
			}
		}
	}

	public void AddGenerateAction(EShipType shipType, int generateTime, global::UnityEngine.Events.UnityAction generateCallBack, out CoolTimeData outData)
	{
		if (!this.coolTimeData.ContainsKey(shipType))
		{
			CoolTimeData coolTimeData = new CoolTimeData();
			coolTimeData.shipType = shipType;
			if (CoolTimeManager.HaveKey(shipType.ToString()))
			{
				global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(shipType.ToString());
				if (leftCoolTime.TotalSeconds < 0.0)
				{
					generateCallBack();
					CoolTimeManager.DeleteCoolTime(shipType.ToString());
					coolTimeData.isActive = false;
					coolTimeData.startTicks = 0L;
					coolTimeData.endTicks = 0L;
				}
				else
				{
					coolTimeData.isActive = true;
					coolTimeData.startTicks = global::System.DateTime.Now.Subtract(leftCoolTime).Ticks;
					coolTimeData.endTicks = global::System.DateTime.Now.Add(leftCoolTime).Ticks;
				}
			}
			else
			{
				coolTimeData.startTicks = 0L;
				coolTimeData.endTicks = 0L;
				coolTimeData.isActive = false;
			}
			coolTimeData.durationTime = generateTime;
			coolTimeData.unityAction = generateCallBack;
			outData = coolTimeData;
			this.coolTimeData.Add(shipType, coolTimeData);
		}
		else
		{
			CoolTimeData coolTimeData2 = this.coolTimeData[shipType];
			coolTimeData2.isActive = true;
			CoolTimeManager.AddCooltimeSecondFromNow(shipType.ToString(), generateTime);
			coolTimeData2.durationTime = generateTime;
			coolTimeData2.startTicks = global::System.DateTime.Now.Ticks;
			coolTimeData2.endTicks = global::System.DateTime.Now.AddSeconds(generateTime).Ticks;
			coolTimeData2.unityAction = generateCallBack;
			outData = coolTimeData2;
			ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(shipType);
			MyData myData = PlayerInfo.Instance.myData;
			myData.reserveShipCount = (int)myData.reserveShipCount + shipBaseData.useCount;
		}
	}

	public void Reset()
	{
		for (int i = 1; i < 14; i++)
		{
			if (coolTimeData.ContainsKey((EShipType)i))
			{
				coolTimeData[(EShipType)i].isActive = false;
				coolTimeData[(EShipType)i].startTicks = 0L;
				coolTimeData[(EShipType)i].endTicks = 0L;
			}
		}
	}
}
