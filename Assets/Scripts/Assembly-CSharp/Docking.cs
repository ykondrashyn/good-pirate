public class Docking : global::UnityEngine.MonoBehaviour
{
	public global::UnityEngine.ParticleSystem bubbleParticle;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Collider collider;

	[global::UnityEngine.HideInInspector]
	public Island myIsland;

	private global::System.Collections.Generic.Dictionary<ShipController, int> overlapCheck = new global::System.Collections.Generic.Dictionary<ShipController, int>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject caveOpenObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject caveCloseObj;

	[global::UnityEngine.SerializeField]
	private TaxShip taxShip;

	[global::UnityEngine.SerializeField]
	private Flag[] flags;

	public TaxShip TaxShip => taxShip;

	private void Awake()
	{
		collider = GetComponent<global::UnityEngine.Collider>();
		taxShip = GetComponentInChildren<TaxShip>();
	}

	private void OnEnable()
	{
		StartCoroutine(CaveCheck());
		RefreshFlag();
	}

	public void RefreshFlag()
	{
		int flagId = PlayerInfo.Instance.myData.selectedFlagId;
		if (FlagManager.Instance.GetFlagInfo(flagId, out var info))
		{
			Flag[] array = flags;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetFlag(info.flagMaterial);
			}
		}
	}

	private global::System.Collections.IEnumerator CaveCheck()
	{
		yield return new global::UnityEngine.WaitUntil(() => myIsland != null);
		bool isClearIslandClaim = PlayerInfo.Instance.GetIsClearIslandClaim(myIsland.status.islandId);
		if (myIsland.status.islandId == 0 || !isClearIslandClaim)
		{
			caveOpenObj.SetActive(value: false);
			caveCloseObj.SetActive(value: true);
		}
		else
		{
			caveOpenObj.SetActive(value: true);
			caveCloseObj.SetActive(value: false);
		}
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		if (GameManager.Instance.IsInIsland)
		{
			return;
		}
		ShipController componentInParent = other.GetComponentInParent<ShipController>();
		if (componentInParent != null)
		{
			if (!overlapCheck.ContainsKey(componentInParent))
			{
				overlapCheck.Add(componentInParent, 1);
				InIsland();
			}
			else
			{
				int num = overlapCheck[componentInParent];
				num++;
				overlapCheck[componentInParent] = num;
			}
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider other)
	{
		ShipController componentInParent = other.GetComponentInParent<ShipController>();
		if (componentInParent != null && overlapCheck.ContainsKey(componentInParent))
		{
			int num = overlapCheck[componentInParent];
			num--;
			overlapCheck[componentInParent] = num;
			if (num <= 0)
			{
				overlapCheck.Clear();
			}
		}
	}

	public void OverlapCheckClear()
	{
		overlapCheck.Clear();
	}

	public void DockingActive(bool active)
	{
		if (!active)
		{
			if ((bool)bubbleParticle)
			{
				bubbleParticle.Stop();
			}
			if ((bool)collider)
			{
				collider.enabled = false;
			}
		}
		else
		{
			if ((bool)collider)
			{
				collider.enabled = true;
			}
			StartCoroutine(ParticleOn());
		}
	}

	private global::System.Collections.IEnumerator ParticleOn()
	{
		yield return null;
		if ((bool)bubbleParticle)
		{
			bubbleParticle.Play();
		}
	}

	private void InIsland()
	{
		if (myIsland.status.islandId == PlayerInfo.Instance.myData.destinationIslandId)
		{
			PlayerInfo.Instance.myData.destinationIslandId = -1;
			GameManager.Instance.playerShipController.SetDestination(null);
			GameManager.Instance.worldManager.RefreshDestination(null);
			PlayerInfo.Instance.SaveData();
		}
		UI_Manager.Instance.ui_enterIslandPopup.gameObject.SetActive(value: true);
		UI_Manager.Instance.ui_enterIslandPopup.SetDock(this, delegate
		{
			DockingActive(active: false);
		});
	}
}
