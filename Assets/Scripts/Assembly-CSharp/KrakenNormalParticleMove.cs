public class KrakenNormalParticleMove : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem particle;

	public void SeaWaveParticleOn()
	{
		particle.gameObject.SetActive(value: true);
		particle.Play();
	}

	public void SeaWaveParticleOff()
	{
		particle.Stop();
		Invoke("SelfInactive", 4.5f);
	}

	private void SelfInactive()
	{
		particle.gameObject.SetActive(value: false);
	}
}
