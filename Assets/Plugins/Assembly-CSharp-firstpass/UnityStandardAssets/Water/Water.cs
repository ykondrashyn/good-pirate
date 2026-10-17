namespace UnityStandardAssets.Water
{
	[global::UnityEngine.ExecuteInEditMode]
	public class Water : global::UnityEngine.MonoBehaviour
	{
		public enum WaterMode
		{
			Simple = 0,
			Reflective = 1,
			Refractive = 2
		}

		public global::UnityStandardAssets.Water.Water.WaterMode waterMode = global::UnityStandardAssets.Water.Water.WaterMode.Refractive;

		public bool disablePixelLights = true;

		public int textureSize = 256;

		public float clipPlaneOffset = 0.07f;

		public global::UnityEngine.LayerMask reflectLayers = -1;

		public global::UnityEngine.LayerMask refractLayers = -1;

		private global::System.Collections.Generic.Dictionary<global::UnityEngine.Camera, global::UnityEngine.Camera> m_ReflectionCameras = new global::System.Collections.Generic.Dictionary<global::UnityEngine.Camera, global::UnityEngine.Camera>();

		private global::System.Collections.Generic.Dictionary<global::UnityEngine.Camera, global::UnityEngine.Camera> m_RefractionCameras = new global::System.Collections.Generic.Dictionary<global::UnityEngine.Camera, global::UnityEngine.Camera>();

		private global::UnityEngine.RenderTexture m_ReflectionTexture;

		private global::UnityEngine.RenderTexture m_RefractionTexture;

		private global::UnityStandardAssets.Water.Water.WaterMode m_HardwareWaterSupport = global::UnityStandardAssets.Water.Water.WaterMode.Refractive;

		private int m_OldReflectionTextureSize;

		private int m_OldRefractionTextureSize;

		private static bool s_InsideWater;

		public void OnWillRenderObject()
		{
			if (!base.enabled || !GetComponent<global::UnityEngine.Renderer>() || !GetComponent<global::UnityEngine.Renderer>().sharedMaterial || !GetComponent<global::UnityEngine.Renderer>().enabled)
			{
				return;
			}
			global::UnityEngine.Camera current = global::UnityEngine.Camera.current;
			if ((bool)current && !s_InsideWater)
			{
				s_InsideWater = true;
				m_HardwareWaterSupport = FindHardwareWaterSupport();
				global::UnityStandardAssets.Water.Water.WaterMode waterMode = GetWaterMode();
				CreateWaterObjects(current, out var reflectionCamera, out var refractionCamera);
				global::UnityEngine.Vector3 position = base.transform.position;
				global::UnityEngine.Vector3 up = base.transform.up;
				int pixelLightCount = global::UnityEngine.QualitySettings.pixelLightCount;
				if (disablePixelLights)
				{
					global::UnityEngine.QualitySettings.pixelLightCount = 0;
				}
				UpdateCameraModes(current, reflectionCamera);
				UpdateCameraModes(current, refractionCamera);
				if (waterMode >= global::UnityStandardAssets.Water.Water.WaterMode.Reflective)
				{
					float w = 0f - global::UnityEngine.Vector3.Dot(up, position) - clipPlaneOffset;
					global::UnityEngine.Vector4 plane = new global::UnityEngine.Vector4(up.x, up.y, up.z, w);
					global::UnityEngine.Matrix4x4 reflectionMat = global::UnityEngine.Matrix4x4.zero;
					CalculateReflectionMatrix(ref reflectionMat, plane);
					global::UnityEngine.Vector3 position2 = current.transform.position;
					global::UnityEngine.Vector3 position3 = reflectionMat.MultiplyPoint(position2);
					reflectionCamera.worldToCameraMatrix = current.worldToCameraMatrix * reflectionMat;
					global::UnityEngine.Vector4 clipPlane = CameraSpacePlane(reflectionCamera, position, up, 1f);
					reflectionCamera.projectionMatrix = current.CalculateObliqueMatrix(clipPlane);
					reflectionCamera.cullingMatrix = current.projectionMatrix * current.worldToCameraMatrix;
					reflectionCamera.cullingMask = -17 & reflectLayers.value;
					reflectionCamera.targetTexture = m_ReflectionTexture;
					bool invertCulling = global::UnityEngine.GL.invertCulling;
					global::UnityEngine.GL.invertCulling = !invertCulling;
					reflectionCamera.transform.position = position3;
					global::UnityEngine.Vector3 eulerAngles = current.transform.eulerAngles;
					reflectionCamera.transform.eulerAngles = new global::UnityEngine.Vector3(0f - eulerAngles.x, eulerAngles.y, eulerAngles.z);
					reflectionCamera.Render();
					reflectionCamera.transform.position = position2;
					global::UnityEngine.GL.invertCulling = invertCulling;
					GetComponent<global::UnityEngine.Renderer>().sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
				}
				if (waterMode >= global::UnityStandardAssets.Water.Water.WaterMode.Refractive)
				{
					refractionCamera.worldToCameraMatrix = current.worldToCameraMatrix;
					global::UnityEngine.Vector4 clipPlane2 = CameraSpacePlane(refractionCamera, position, up, -1f);
					refractionCamera.projectionMatrix = current.CalculateObliqueMatrix(clipPlane2);
					refractionCamera.cullingMatrix = current.projectionMatrix * current.worldToCameraMatrix;
					refractionCamera.cullingMask = -17 & refractLayers.value;
					refractionCamera.targetTexture = m_RefractionTexture;
					refractionCamera.transform.position = current.transform.position;
					refractionCamera.transform.rotation = current.transform.rotation;
					refractionCamera.Render();
					GetComponent<global::UnityEngine.Renderer>().sharedMaterial.SetTexture("_RefractionTex", m_RefractionTexture);
				}
				if (disablePixelLights)
				{
					global::UnityEngine.QualitySettings.pixelLightCount = pixelLightCount;
				}
				switch (waterMode)
				{
				case global::UnityStandardAssets.Water.Water.WaterMode.Simple:
					global::UnityEngine.Shader.EnableKeyword("WATER_SIMPLE");
					global::UnityEngine.Shader.DisableKeyword("WATER_REFLECTIVE");
					global::UnityEngine.Shader.DisableKeyword("WATER_REFRACTIVE");
					break;
				case global::UnityStandardAssets.Water.Water.WaterMode.Reflective:
					global::UnityEngine.Shader.DisableKeyword("WATER_SIMPLE");
					global::UnityEngine.Shader.EnableKeyword("WATER_REFLECTIVE");
					global::UnityEngine.Shader.DisableKeyword("WATER_REFRACTIVE");
					break;
				case global::UnityStandardAssets.Water.Water.WaterMode.Refractive:
					global::UnityEngine.Shader.DisableKeyword("WATER_SIMPLE");
					global::UnityEngine.Shader.DisableKeyword("WATER_REFLECTIVE");
					global::UnityEngine.Shader.EnableKeyword("WATER_REFRACTIVE");
					break;
				}
				s_InsideWater = false;
			}
		}

		private void OnDisable()
		{
			if ((bool)m_ReflectionTexture)
			{
				global::UnityEngine.Object.DestroyImmediate(m_ReflectionTexture);
				m_ReflectionTexture = null;
			}
			if ((bool)m_RefractionTexture)
			{
				global::UnityEngine.Object.DestroyImmediate(m_RefractionTexture);
				m_RefractionTexture = null;
			}
			foreach (global::System.Collections.Generic.KeyValuePair<global::UnityEngine.Camera, global::UnityEngine.Camera> reflectionCamera in m_ReflectionCameras)
			{
				global::UnityEngine.Object.DestroyImmediate(reflectionCamera.Value.gameObject);
			}
			m_ReflectionCameras.Clear();
			foreach (global::System.Collections.Generic.KeyValuePair<global::UnityEngine.Camera, global::UnityEngine.Camera> refractionCamera in m_RefractionCameras)
			{
				global::UnityEngine.Object.DestroyImmediate(refractionCamera.Value.gameObject);
			}
			m_RefractionCameras.Clear();
		}

		private void Update()
		{
			if ((bool)GetComponent<global::UnityEngine.Renderer>())
			{
				global::UnityEngine.Material sharedMaterial = GetComponent<global::UnityEngine.Renderer>().sharedMaterial;
				if ((bool)sharedMaterial)
				{
					global::UnityEngine.Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
					float num = sharedMaterial.GetFloat("_WaveScale");
					global::UnityEngine.Vector4 value = new global::UnityEngine.Vector4(num, num, num * 0.4f, num * 0.45f);
					double num2 = (double)global::UnityEngine.Time.timeSinceLevelLoad / 20.0;
					global::UnityEngine.Vector4 value2 = new global::UnityEngine.Vector4((float)global::System.Math.IEEERemainder((double)(vector.x * value.x) * num2, 1.0), (float)global::System.Math.IEEERemainder((double)(vector.y * value.y) * num2, 1.0), (float)global::System.Math.IEEERemainder((double)(vector.z * value.z) * num2, 1.0), (float)global::System.Math.IEEERemainder((double)(vector.w * value.w) * num2, 1.0));
					sharedMaterial.SetVector("_WaveOffset", value2);
					sharedMaterial.SetVector("_WaveScale4", value);
				}
			}
		}

		private void UpdateCameraModes(global::UnityEngine.Camera src, global::UnityEngine.Camera dest)
		{
			if (dest == null)
			{
				return;
			}
			dest.clearFlags = src.clearFlags;
			dest.backgroundColor = src.backgroundColor;
			if (src.clearFlags == global::UnityEngine.CameraClearFlags.Skybox)
			{
				global::UnityEngine.Skybox component = src.GetComponent<global::UnityEngine.Skybox>();
				global::UnityEngine.Skybox component2 = dest.GetComponent<global::UnityEngine.Skybox>();
				if (!component || !component.material)
				{
					component2.enabled = false;
				}
				else
				{
					component2.enabled = true;
					component2.material = component.material;
				}
			}
			dest.farClipPlane = src.farClipPlane;
			dest.nearClipPlane = src.nearClipPlane;
			dest.orthographic = src.orthographic;
			dest.fieldOfView = src.fieldOfView;
			dest.aspect = src.aspect;
			dest.orthographicSize = src.orthographicSize;
		}

		private void CreateWaterObjects(global::UnityEngine.Camera currentCamera, out global::UnityEngine.Camera reflectionCamera, out global::UnityEngine.Camera refractionCamera)
		{
			global::UnityStandardAssets.Water.Water.WaterMode waterMode = GetWaterMode();
			reflectionCamera = null;
			refractionCamera = null;
			if (waterMode >= global::UnityStandardAssets.Water.Water.WaterMode.Reflective)
			{
				if (!m_ReflectionTexture || m_OldReflectionTextureSize != textureSize)
				{
					if ((bool)m_ReflectionTexture)
					{
						global::UnityEngine.Object.DestroyImmediate(m_ReflectionTexture);
					}
					m_ReflectionTexture = new global::UnityEngine.RenderTexture(textureSize, textureSize, 16);
					m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
					m_ReflectionTexture.isPowerOfTwo = true;
					m_ReflectionTexture.hideFlags = global::UnityEngine.HideFlags.DontSave;
					m_OldReflectionTextureSize = textureSize;
				}
				m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
				if (!reflectionCamera)
				{
					global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(global::UnityEngine.Camera), typeof(global::UnityEngine.Skybox));
					reflectionCamera = gameObject.GetComponent<global::UnityEngine.Camera>();
					reflectionCamera.enabled = false;
					reflectionCamera.transform.position = base.transform.position;
					reflectionCamera.transform.rotation = base.transform.rotation;
					reflectionCamera.gameObject.AddComponent<global::UnityEngine.FlareLayer>();
					gameObject.hideFlags = global::UnityEngine.HideFlags.HideAndDontSave;
					m_ReflectionCameras[currentCamera] = reflectionCamera;
				}
			}
			if (waterMode < global::UnityStandardAssets.Water.Water.WaterMode.Refractive)
			{
				return;
			}
			if (!m_RefractionTexture || m_OldRefractionTextureSize != textureSize)
			{
				if ((bool)m_RefractionTexture)
				{
					global::UnityEngine.Object.DestroyImmediate(m_RefractionTexture);
				}
				m_RefractionTexture = new global::UnityEngine.RenderTexture(textureSize, textureSize, 16);
				m_RefractionTexture.name = "__WaterRefraction" + GetInstanceID();
				m_RefractionTexture.isPowerOfTwo = true;
				m_RefractionTexture.hideFlags = global::UnityEngine.HideFlags.DontSave;
				m_OldRefractionTextureSize = textureSize;
			}
			m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
			if (!refractionCamera)
			{
				global::UnityEngine.GameObject gameObject2 = new global::UnityEngine.GameObject("Water Refr Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(global::UnityEngine.Camera), typeof(global::UnityEngine.Skybox));
				refractionCamera = gameObject2.GetComponent<global::UnityEngine.Camera>();
				refractionCamera.enabled = false;
				refractionCamera.transform.position = base.transform.position;
				refractionCamera.transform.rotation = base.transform.rotation;
				refractionCamera.gameObject.AddComponent<global::UnityEngine.FlareLayer>();
				gameObject2.hideFlags = global::UnityEngine.HideFlags.HideAndDontSave;
				m_RefractionCameras[currentCamera] = refractionCamera;
			}
		}

		private global::UnityStandardAssets.Water.Water.WaterMode GetWaterMode()
		{
			if (m_HardwareWaterSupport < waterMode)
			{
				return m_HardwareWaterSupport;
			}
			return waterMode;
		}

		private global::UnityStandardAssets.Water.Water.WaterMode FindHardwareWaterSupport()
		{
			if (!GetComponent<global::UnityEngine.Renderer>())
			{
				return global::UnityStandardAssets.Water.Water.WaterMode.Simple;
			}
			global::UnityEngine.Material sharedMaterial = GetComponent<global::UnityEngine.Renderer>().sharedMaterial;
			if (!sharedMaterial)
			{
				return global::UnityStandardAssets.Water.Water.WaterMode.Simple;
			}
			string text = sharedMaterial.GetTag("WATERMODE", searchFallbacks: false);
			if (text == "Refractive")
			{
				return global::UnityStandardAssets.Water.Water.WaterMode.Refractive;
			}
			if (text == "Reflective")
			{
				return global::UnityStandardAssets.Water.Water.WaterMode.Reflective;
			}
			return global::UnityStandardAssets.Water.Water.WaterMode.Simple;
		}

		private global::UnityEngine.Vector4 CameraSpacePlane(global::UnityEngine.Camera cam, global::UnityEngine.Vector3 pos, global::UnityEngine.Vector3 normal, float sideSign)
		{
			global::UnityEngine.Vector3 point = pos + normal * clipPlaneOffset;
			global::UnityEngine.Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			global::UnityEngine.Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
			global::UnityEngine.Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new global::UnityEngine.Vector4(rhs.x, rhs.y, rhs.z, 0f - global::UnityEngine.Vector3.Dot(lhs, rhs));
		}

		private static void CalculateReflectionMatrix(ref global::UnityEngine.Matrix4x4 reflectionMat, global::UnityEngine.Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
			reflectionMat.m01 = -2f * plane[0] * plane[1];
			reflectionMat.m02 = -2f * plane[0] * plane[2];
			reflectionMat.m03 = -2f * plane[3] * plane[0];
			reflectionMat.m10 = -2f * plane[1] * plane[0];
			reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
			reflectionMat.m12 = -2f * plane[1] * plane[2];
			reflectionMat.m13 = -2f * plane[3] * plane[1];
			reflectionMat.m20 = -2f * plane[2] * plane[0];
			reflectionMat.m21 = -2f * plane[2] * plane[1];
			reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
			reflectionMat.m23 = -2f * plane[3] * plane[2];
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
		}
	}
}
