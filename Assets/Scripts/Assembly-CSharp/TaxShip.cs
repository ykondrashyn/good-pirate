public class TaxShip : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject taxBoxSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform taxPivot;

	private RootBox rootBox;

	[global::UnityEngine.SerializeField]
	private bool isNotMove;

	private bool isThrewRootBox;

	public ShipBase myShip { get; private set; }

	public void MakeTaxBox(params Reward[] rewards)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(taxBoxSrc, taxPivot);
		rootBox = gameObject.GetComponent<RootBox>();
		rootBox.AddReward(rewards);
		rootBox.transform.localPosition = global::UnityEngine.Vector3.zero;
		rootBox.transform.localRotation = global::UnityEngine.Quaternion.identity;
		rootBox.transform.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(rootBox.transform, 1f, 0.6f), global::DG.Tweening.Ease.OutBack);
	}

	public void Force()
	{
		if (!(rootBox == null))
		{
			rootBox.transform.SetParent(null);
			rootBox.ForceRoot();
			rootBox = null;
		}
	}

	private void OnEnable()
	{
		if (myShip == null)
		{
			myShip = GetComponent<ShipBase>();
		}
		if ((bool)myShip)
		{
			myShip.accelerationFactor = 1.5f;
		}
		isThrewRootBox = false;
		StartCoroutine(MoveRutine());
	}

	private global::System.Collections.IEnumerator MoveRutine()
	{
		if (isNotMove)
		{
			yield break;
		}
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		yield return wait;
		yield return wait;
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::UnityEngine.Vector3 goBackPos = global::UnityEngine.Vector3.zero;
		while (true)
		{
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			global::UnityEngine.Vector3 vector;
			if (!isThrewRootBox)
			{
				vector = playerShip.transform.position - base.transform.position;
			}
			else
			{
				if (goBackPos == global::UnityEngine.Vector3.zero)
				{
					global::UnityEngine.Vector3 vector2 = playerShip.transform.position - base.transform.position;
					goBackPos = base.transform.position - vector2.normalized * 100f;
				}
				vector = goBackPos - base.transform.position;
			}
			float angleForward = myMathf.GetAngleForward(vector.normalized);
			myShip.SetTargetAngle(angleForward);
			myShip.Acceleration(myShip.acceleration);
			if (!isThrewRootBox)
			{
				if (vector.magnitude <= 35f)
				{
					Force();
					isThrewRootBox = true;
				}
			}
			else if (vector.magnitude <= 10f)
			{
				base.gameObject.SetActive(value: false);
			}
			yield return wait;
		}
	}
}
