namespace UnityStandardAssets.Water
{
	[global::UnityEngine.ExecuteInEditMode]
	public class WaterBase : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.Material sharedMaterial;

		public global::UnityStandardAssets.Water.WaterQuality waterQuality = global::UnityStandardAssets.Water.WaterQuality.High;

		public bool edgeBlend = true;

		public void UpdateShader()
		{
			if (waterQuality > global::UnityStandardAssets.Water.WaterQuality.Medium)
			{
				sharedMaterial.shader.maximumLOD = 501;
			}
			else if (waterQuality > global::UnityStandardAssets.Water.WaterQuality.Low)
			{
				sharedMaterial.shader.maximumLOD = 301;
			}
			else
			{
				sharedMaterial.shader.maximumLOD = 201;
			}
			if (!global::UnityEngine.SystemInfo.SupportsRenderTextureFormat(global::UnityEngine.RenderTextureFormat.Depth))
			{
				edgeBlend = false;
			}
			if (edgeBlend)
			{
				global::UnityEngine.Shader.EnableKeyword("WATER_EDGEBLEND_ON");
				global::UnityEngine.Shader.DisableKeyword("WATER_EDGEBLEND_OFF");
				if ((bool)global::UnityEngine.Camera.main)
				{
					global::UnityEngine.Camera.main.depthTextureMode |= global::UnityEngine.DepthTextureMode.Depth;
				}
			}
			else
			{
				global::UnityEngine.Shader.EnableKeyword("WATER_EDGEBLEND_OFF");
				global::UnityEngine.Shader.DisableKeyword("WATER_EDGEBLEND_ON");
			}
		}

		public void WaterTileBeingRendered(global::UnityEngine.Transform tr, global::UnityEngine.Camera currentCam)
		{
			if ((bool)currentCam && edgeBlend)
			{
				currentCam.depthTextureMode |= global::UnityEngine.DepthTextureMode.Depth;
			}
		}

		public void Update()
		{
			if ((bool)sharedMaterial)
			{
				UpdateShader();
			}
		}
	}
}
