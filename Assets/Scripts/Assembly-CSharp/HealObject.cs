public class HealObject : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Vector3 startPos;

	private global::UnityEngine.Vector3 destPos;

	private float vz;

	private float vx;

	private float vy;

	private float g;

	private float dat;

	private float mh;

	private float dh;

	private float t;

	private float mht = 0.4f;

	private float MAX_Y = 5f;

	private bool isTriggerOn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem myParticle;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem particleBuster;

	private Health target;

	public int healAmount;

	private void PreCalculate()
	{
		dh = destPos.y - startPos.y;
		mh = MAX_Y - startPos.y;
		g = 2f * mh / (mht * mht);
		vy = global::UnityEngine.Mathf.Sqrt(2f * g * mh);
		float num = g;
		float num2 = -2f * vy;
		float num3 = 2f * dh;
		dat = (0f - num2 + global::UnityEngine.Mathf.Sqrt(num2 * num2 - 4f * num * num3)) / (2f * num);
		vx = (0f - (startPos.x - destPos.x)) / dat;
		vz = (0f - (startPos.z - destPos.z)) / dat;
	}

	private global::UnityEngine.Vector3 Move()
	{
		t += global::UnityEngine.Time.deltaTime;
		if (t > dat)
		{
			return global::UnityEngine.Vector3.zero;
		}
		float x = startPos.x + vx * t;
		float z = startPos.z + vz * t;
		float y = startPos.y + vy * t - 0.5f * g * t * t;
		return new global::UnityEngine.Vector3(x, y, z);
	}

	public void Fire(Health target)
	{
		startPos = base.transform.position;
		this.target = target;
		destPos = target.transform.position;
		myParticle.Play();
		PreCalculate();
		StartCoroutine(Parabola());
	}

	private global::System.Collections.IEnumerator Parabola()
	{
		t = 0f;
		while (true)
		{
			global::UnityEngine.Vector3 vector = Move();
			if (!(vector != global::UnityEngine.Vector3.zero))
			{
				break;
			}
			base.transform.position = vector;
			yield return null;
		}
		myParticle.Stop();
		particleBuster.Emit(25);
		target.Heal(healAmount);
		yield return new global::UnityEngine.WaitForSeconds(2f);
		base.gameObject.SetActive(value: false);
	}
}
