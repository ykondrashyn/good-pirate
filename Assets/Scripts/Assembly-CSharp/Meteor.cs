public class Meteor : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform meteorTrans;

	[global::UnityEngine.SerializeField]
	private float durationTime = 3f;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem myParticle;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject exploParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject waterSplashParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform warnTrans;

	public int damage;

	private Health selectedEnemyHealth;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		meteorTrans.gameObject.SetActive(value: false);
		if ((bool)waterSplashParticleObj)
		{
			waterSplashParticleObj.SetActive(value: false);
		}
		if (warnTrans != null)
		{
			warnTrans.gameObject.SetActive(value: false);
		}
	}

	private void LateUpdate()
	{
		transform.rotation = global::UnityEngine.Quaternion.identity;
	}

	public void SetHealth(Health enemyHealth)
	{
		selectedEnemyHealth = enemyHealth;
	}

	public void StartFallingDown(float delay)
	{
		StartCoroutine(FallingDown(delay));
	}

	private global::System.Collections.IEnumerator FallingDown(float delay)
	{
		yield return new global::UnityEngine.WaitForSeconds(delay);
		AudioManager.Instance.PlaySFX("skill_windblast");
		if (warnTrans != null)
		{
			warnTrans.gameObject.SetActive(value: true);
			warnTrans.localScale = global::UnityEngine.Vector3.zero;
			global::DG.Tweening.ShortcutExtensions.DOScale(warnTrans, 1f, 0.6f);
		}
		meteorTrans.gameObject.SetActive(value: true);
		global::UnityEngine.Vector3 startLocalPos = meteorTrans.localPosition;
		float totalTime = 0f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / durationTime;
			global::UnityEngine.Vector3 localPosition = global::UnityEngine.Vector3.Lerp(startLocalPos, global::UnityEngine.Vector3.zero, num);
			meteorTrans.localPosition = localPosition;
			if (num >= 1f)
			{
				break;
			}
			yield return null;
		}
		if (warnTrans != null)
		{
			warnTrans.gameObject.SetActive(value: false);
		}
		myParticle.Stop(withChildren: true);
		if (selectedEnemyHealth != null)
		{
			selectedEnemyHealth.Hit(damage);
			CameraShakeManager.Instance.CameraShake(0.2f, 1f);
		}
		if ((bool)waterSplashParticleObj)
		{
			waterSplashParticleObj.SetActive(value: true);
		}
		if (exploParticleObj != null)
		{
			exploParticleObj.SetActive(value: true);
			yield return new global::UnityEngine.WaitForSeconds(1f);
		}
		yield return new global::UnityEngine.WaitForSeconds(1f);
		global::UnityEngine.Object.Destroy(base.gameObject);
	}
}
