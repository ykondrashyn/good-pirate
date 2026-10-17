[global::UnityEngine.ExecuteInEditMode]
public class particleColorChanger : global::UnityEngine.MonoBehaviour
{
	[global::System.Serializable]
	public class colorChange
	{
		public string Name;

		public global::UnityEngine.ParticleSystem[] colored_ParticleSystem;

		public global::UnityEngine.Gradient customer_Gradient;
	}

	public particleColorChanger.colorChange[] colorChangeList;

	private void Update()
	{
		for (int i = 0; i < colorChangeList.Length; i++)
		{
			for (int j = 0; j < colorChangeList[i].colored_ParticleSystem.Length; j++)
			{
				global::UnityEngine.ParticleSystem.ColorOverLifetimeModule colorOverLifetime = colorChangeList[i].colored_ParticleSystem[j].colorOverLifetime;
				colorOverLifetime.color = colorChangeList[i].customer_Gradient;
			}
		}
	}
}
