namespace UnityStandardAssets.Water
{
	[global::UnityEngine.ExecuteInEditMode]
	[global::UnityEngine.RequireComponent(typeof(global::UnityStandardAssets.Water.WaterBase))]
	public class Displace : global::UnityEngine.MonoBehaviour
	{
		public void Awake()
		{
			if (base.enabled)
			{
				OnEnable();
			}
			else
			{
				OnDisable();
			}
		}

		public void OnEnable()
		{
			global::UnityEngine.Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_ON");
			global::UnityEngine.Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");
		}

		public void OnDisable()
		{
			global::UnityEngine.Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");
			global::UnityEngine.Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_ON");
		}
	}
}
