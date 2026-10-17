public class SharkProjectile : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.SpriteRenderer warningSr;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject waterSplashObj;

	[global::UnityEngine.SerializeField]
	private Hit hitObj;

	private void OnEnable()
	{
		waterSplashObj.SetActive(value: false);
		float x = warningSr.transform.localScale.x;
		warningSr.transform.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.ShortcutExtensions.DOScale(warningSr.transform, x, 1f);
		global::UnityEngine.Color color = warningSr.color;
		float a = color.a;
		color.a = 0f;
		warningSr.color = color;
		color.a = a;
		global::DG.Tweening.DOTweenModuleSprite.DOColor(warningSr, color, 0.5f);
		modelTrans.localPosition = new global::UnityEngine.Vector3(0f, 250f, 0f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, -2.5f, 2f), 1.5f), global::DG.Tweening.Ease.InQuart), delegate
		{
			CollierOn();
		});
		hitObj.gameObject.SetActive(value: false);
	}

	public void SetDamage(int damage)
	{
		hitObj.damage = damage;
	}

	private void CollierOn()
	{
		AudioManager.Instance.PlaySFX("ship_spawn");
		waterSplashObj.SetActive(value: true);
		hitObj.gameObject.SetActive(value: true);
		global::UnityEngine.Object.Destroy(base.gameObject, 2f);
		warningSr.gameObject.SetActive(value: false);
	}
}
