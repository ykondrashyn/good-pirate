public class LifeBead : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Collider myCollider;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private void OnEnable()
	{
		myCollider.enabled = false;
		if ((bool)modelTrans)
		{
			modelTrans.localScale = global::UnityEngine.Vector3.zero;
		}
	}

	public void SetMove(global::UnityEngine.Vector3 pos)
	{
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOMove(transform, pos, 1f), global::DG.Tweening.Ease.Linear), delegate
		{
			myCollider.enabled = true;
		});
		global::DG.Tweening.ShortcutExtensions.DOScale(modelTrans, 1f, 0.6f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalMoveY(modelTrans, 9f, 0.6f), global::DG.Tweening.Ease.OutQuad), delegate
		{
			global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalMoveY(modelTrans, 3.8f, 0.6f), global::DG.Tweening.Ease.InQuad);
		});
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		ShipController shipController = other.GetComponent<ShipController>();
		if (shipController == null)
		{
			shipController = other.GetComponentInParent<ShipController>();
		}
		if (!(shipController == null))
		{
			myCollider.enabled = false;
			StartCoroutine(MoveToTarget(shipController.transform));
		}
	}

	private global::System.Collections.IEnumerator MoveToTarget(global::UnityEngine.Transform targetTrans)
	{
		float totalTime = 0f;
		float duration = 0.3f;
		global::UnityEngine.Vector3 startPos = transform.position;
		global::DG.Tweening.ShortcutExtensions.DOScale(modelTrans, 0f, 0.3f);
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / duration;
			transform.position = global::UnityEngine.Vector3.Lerp(startPos, targetTrans.position, num);
			if (num >= 1f)
			{
				break;
			}
			yield return null;
		}
		HealParticle();
	}

	private void HealParticle()
	{
		float num = 0.1f;
		int heal = (int)((float)GameManager.Instance.playerShip.myHealth.HP_MAX * num);
		GameManager.Instance.playerShip.myHealth.Heal(heal);
		GameManager.Instance.GenerateHealParticle(GameManager.Instance.playerShip.transform);
		foreach (ShipSubController subShip in GameManager.Instance.playerShipController.subShipList)
		{
			heal = (int)((float)subShip.myShip.myHealth.HP_MAX * num);
			subShip.myShip.myHealth.Heal(heal);
			GameManager.Instance.GenerateHealParticle(subShip.transform);
		}
	}
}
