public class TurtleFlower : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject chargingParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject beamParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem gasParticleBuster;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject gasSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject circleWarningObj;

	public int damage;

	[global::UnityEngine.SerializeField]
	private HitContinuous myHit;

	public global::UnityEngine.Animator anim { get; private set; }

	private void Awake()
	{
		anim = GetComponent<global::UnityEngine.Animator>();
	}

	public void SetDamage(int dmg)
	{
		damage = dmg;
		myHit.damage = dmg;
	}

	public void SetTrigger(string str)
	{
		anim.SetTrigger(str);
	}

	public void ChargingOn()
	{
		chargingParticleObj.SetActive(value: true);
	}

	public void BeamOn()
	{
		beamParticleObj.SetActive(value: true);
		AudioManager.Instance.PlaySFX("turtle_solarbeam");
	}

	public void Off()
	{
		chargingParticleObj.SetActive(value: false);
		beamParticleObj.SetActive(value: false);
	}

	public void TurtleGasWarning()
	{
		circleWarningObj.SetActive(value: true);
	}

	public void TurtleGasParticle()
	{
		AudioManager.Instance.PlaySFX("turtle_gas");
		circleWarningObj.SetActive(value: false);
		gasParticleBuster.Emit(40);
		global::UnityEngine.Vector3 position = base.transform.position;
		position.y = 0f;
		HitContinuous component = global::UnityEngine.Object.Instantiate(gasSrc, position, global::UnityEngine.Quaternion.identity).GetComponent<HitContinuous>();
		component.damage = damage;
		component.interval = 0.7f;
	}
}
