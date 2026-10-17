public class KrakenHammerParticle : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject particleObj;

	public void ParticleOn()
	{
		AudioManager.Instance.PlaySFX("boss_swipe");
		particleObj.SetActive(value: true);
		AudioManager.Instance.PlaySFX("boss_water_splash");
	}
}
