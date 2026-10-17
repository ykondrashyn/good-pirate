namespace UnityStandardAssets.Water
{
	[global::UnityEngine.ExecuteInEditMode]
	public class WaterBasic : global::UnityEngine.MonoBehaviour
	{
		private void Update()
		{
			global::UnityEngine.Renderer component = GetComponent<global::UnityEngine.Renderer>();
			if ((bool)component)
			{
				global::UnityEngine.Material sharedMaterial = component.sharedMaterial;
				if ((bool)sharedMaterial)
				{
					global::UnityEngine.Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
					float num = sharedMaterial.GetFloat("_WaveScale");
					float num2 = global::UnityEngine.Time.time / 20f;
					global::UnityEngine.Vector4 vector2 = vector * (num2 * num);
					global::UnityEngine.Vector4 value = new global::UnityEngine.Vector4(global::UnityEngine.Mathf.Repeat(vector2.x, 1f), global::UnityEngine.Mathf.Repeat(vector2.y, 1f), global::UnityEngine.Mathf.Repeat(vector2.z, 1f), global::UnityEngine.Mathf.Repeat(vector2.w, 1f));
					sharedMaterial.SetVector("_WaveOffset", value);
				}
			}
		}
	}
}
