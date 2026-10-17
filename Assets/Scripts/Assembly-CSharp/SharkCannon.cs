public class SharkCannon : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject projectileSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject hitProjectile;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform[] cannonTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform smokeTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem smokeParticle;

	public int damage;

	public void Shoot()
	{
		StartCoroutine("ShootRutine");
	}

	private global::System.Collections.IEnumerator ShootRutine()
	{
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.4f);
		int shootCount = global::UnityEngine.Random.Range(3, 7);
		int shootCannonNum = 0;
		for (int i = 0; i < shootCount; i++)
		{
			AudioManager.Instance.PlaySFX("EXPLOSION_Recorded_03_stereo");
			global::UnityEngine.Vector3 position = cannonTrans[shootCannonNum].position;
			global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(projectileSrc, position, global::UnityEngine.Quaternion.identity);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOMoveY(obj.transform, 250f, 2f), delegate
			{
				global::UnityEngine.Object.Destroy(obj, 0.5f);
			});
			smokeTrans.transform.position = position;
			smokeParticle.Emit(15);
			int num = shootCannonNum + 1;
			shootCannonNum = num;
			if (num >= cannonTrans.Length)
			{
				shootCannonNum = 0;
			}
			yield return wait;
		}
		yield return new global::UnityEngine.WaitForSeconds(1.5f);
		if (GameManager.Instance.playerShip == null || !GameManager.Instance.IsBossFighting || GameManager.Instance.IsInIsland)
		{
			yield break;
		}
		for (int i = 0; i < shootCount; i++)
		{
			if (GameManager.Instance.playerShip != null)
			{
				global::UnityEngine.Transform transform = GameManager.Instance.playerShip.transform;
				global::UnityEngine.Vector3 position2 = transform.position;
				position2 += transform.forward * global::UnityEngine.Random.Range(-15f, 40f);
				position2 += transform.right * global::UnityEngine.Random.Range(-25f, 25f);
				position2.y = 0f;
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(hitProjectile, position2, global::UnityEngine.Quaternion.identity);
				gameObject.GetComponent<SharkProjectile>().SetDamage(damage);
				GameManager.Instance.AddObject(gameObject);
				yield return wait;
			}
		}
		yield return null;
	}
}
