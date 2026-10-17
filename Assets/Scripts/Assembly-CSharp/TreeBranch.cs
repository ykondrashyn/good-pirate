public class TreeBranch : global::UnityEngine.MonoBehaviour
{
	private Hit myHit;

	private void Awake()
	{
		myHit = GetComponent<Hit>();
	}

	private void Start()
	{
		base.transform.localScale = new global::UnityEngine.Vector3(1f, 1f, 0f);
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, global::UnityEngine.Vector3.one, 0.6f), global::DG.Tweening.Ease.OutExpo);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, new global::UnityEngine.Vector3(1f, 1f, 0f), 0.2f), 2f), delegate
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	public void SetDamage(int dmg)
	{
		myHit.damage = dmg;
	}
}
