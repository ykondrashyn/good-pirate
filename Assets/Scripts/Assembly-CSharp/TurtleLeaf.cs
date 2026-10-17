public class TurtleLeaf : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Animator anim;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject particleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Collider collider;

	private global::UnityEngine.GameObject warningObj;

	[global::UnityEngine.SerializeField]
	private Hit myHit;

	[global::UnityEngine.ContextMenu("get")]
	private void Awake()
	{
		collider = GetComponent<global::UnityEngine.Collider>();
		anim = GetComponent<global::UnityEngine.Animator>();
		particleObj = base.transform.Find("WaterSplash03").gameObject;
		collider = base.transform.Find("WaterSplash03").gameObject.GetComponent<global::UnityEngine.Collider>();
		warningObj = base.transform.Find("warning").gameObject;
		particleObj.SetActive(value: false);
		if ((bool)warningObj)
		{
			warningObj.SetActive(value: false);
		}
	}

	public void Attack()
	{
		anim.SetTrigger("Attack");
		if ((bool)warningObj)
		{
			warningObj.SetActive(value: true);
		}
	}

	public void SetDamage(int dmg)
	{
		myHit.damage = dmg;
	}

	public void AttackOn()
	{
		AudioManager.Instance.PlaySFX("boss_swipe");
		if ((bool)warningObj)
		{
			warningObj.SetActive(value: false);
		}
		particleObj.SetActive(value: true);
		if (!collider.enabled)
		{
			collider.enabled = true;
		}
	}

	public void ColliderOff()
	{
		if (collider.enabled)
		{
			collider.enabled = false;
		}
	}

	public void AttackObjectOff()
	{
		particleObj.SetActive(value: false);
	}
}
