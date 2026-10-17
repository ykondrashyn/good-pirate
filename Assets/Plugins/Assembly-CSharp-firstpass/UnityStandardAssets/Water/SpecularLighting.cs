namespace UnityStandardAssets.Water
{
	[global::UnityEngine.RequireComponent(typeof(global::UnityStandardAssets.Water.WaterBase))]
	[global::UnityEngine.ExecuteInEditMode]
	public class SpecularLighting : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.Transform specularLight;

		private global::UnityStandardAssets.Water.WaterBase m_WaterBase;

		public void Start()
		{
			m_WaterBase = (global::UnityStandardAssets.Water.WaterBase)base.gameObject.GetComponent(typeof(global::UnityStandardAssets.Water.WaterBase));
		}

		public void Update()
		{
			if (!m_WaterBase)
			{
				m_WaterBase = (global::UnityStandardAssets.Water.WaterBase)base.gameObject.GetComponent(typeof(global::UnityStandardAssets.Water.WaterBase));
			}
			if ((bool)specularLight && (bool)m_WaterBase.sharedMaterial)
			{
				m_WaterBase.sharedMaterial.SetVector("_WorldLightDir", specularLight.transform.forward);
			}
		}
	}
}
