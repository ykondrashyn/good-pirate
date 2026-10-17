namespace UnityStandardAssets.Water
{
	[global::UnityEngine.ExecuteInEditMode]
	[global::UnityEngine.RequireComponent(typeof(global::UnityStandardAssets.Water.WaterBase))]
	public class PlanarReflection : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.LayerMask reflectionMask;

		public bool reflectSkybox;

		public global::UnityEngine.Color clearColor = global::UnityEngine.Color.grey;

		public string reflectionSampler = "_ReflectionTex";

		public float clipPlaneOffset = 0.07f;

		private global::UnityEngine.Vector3 m_Oldpos;

		private global::UnityEngine.Camera m_ReflectionCamera;

		private global::UnityEngine.Material m_SharedMaterial;

		private global::System.Collections.Generic.Dictionary<global::UnityEngine.Camera, bool> m_HelperCameras;

		public void Start()
		{
			m_SharedMaterial = ((global::UnityStandardAssets.Water.WaterBase)base.gameObject.GetComponent(typeof(global::UnityStandardAssets.Water.WaterBase))).sharedMaterial;
		}

		private global::UnityEngine.Camera CreateReflectionCameraFor(global::UnityEngine.Camera cam)
		{
			string text = base.gameObject.name + "Reflection" + cam.name;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.GameObject.Find(text);
			if (!gameObject)
			{
				gameObject = new global::UnityEngine.GameObject(text, typeof(global::UnityEngine.Camera));
			}
			if (!gameObject.GetComponent(typeof(global::UnityEngine.Camera)))
			{
				gameObject.AddComponent(typeof(global::UnityEngine.Camera));
			}
			global::UnityEngine.Camera component = gameObject.GetComponent<global::UnityEngine.Camera>();
			component.backgroundColor = clearColor;
			component.clearFlags = (reflectSkybox ? global::UnityEngine.CameraClearFlags.Skybox : global::UnityEngine.CameraClearFlags.Color);
			SetStandardCameraParameter(component, reflectionMask);
			if (!component.targetTexture)
			{
				component.targetTexture = CreateTextureFor(cam);
			}
			return component;
		}

		private void SetStandardCameraParameter(global::UnityEngine.Camera cam, global::UnityEngine.LayerMask mask)
		{
			cam.cullingMask = (int)mask & ~(1 << global::UnityEngine.LayerMask.NameToLayer("Water"));
			cam.backgroundColor = global::UnityEngine.Color.black;
			cam.enabled = false;
		}

		private global::UnityEngine.RenderTexture CreateTextureFor(global::UnityEngine.Camera cam)
		{
			return new global::UnityEngine.RenderTexture(global::UnityEngine.Mathf.FloorToInt((float)cam.pixelWidth * 0.5f), global::UnityEngine.Mathf.FloorToInt((float)cam.pixelHeight * 0.5f), 24)
			{
				hideFlags = global::UnityEngine.HideFlags.DontSave
			};
		}

		public void RenderHelpCameras(global::UnityEngine.Camera currentCam)
		{
			if (m_HelperCameras == null)
			{
				m_HelperCameras = new global::System.Collections.Generic.Dictionary<global::UnityEngine.Camera, bool>();
			}
			if (!m_HelperCameras.ContainsKey(currentCam))
			{
				m_HelperCameras.Add(currentCam, value: false);
			}
			if (!m_HelperCameras[currentCam])
			{
				if (!m_ReflectionCamera)
				{
					m_ReflectionCamera = CreateReflectionCameraFor(currentCam);
				}
				RenderReflectionFor(currentCam, m_ReflectionCamera);
				m_HelperCameras[currentCam] = true;
			}
		}

		public void LateUpdate()
		{
			if (m_HelperCameras != null)
			{
				m_HelperCameras.Clear();
			}
		}

		public void WaterTileBeingRendered(global::UnityEngine.Transform tr, global::UnityEngine.Camera currentCam)
		{
			RenderHelpCameras(currentCam);
			if ((bool)m_ReflectionCamera && (bool)m_SharedMaterial)
			{
				m_SharedMaterial.SetTexture(reflectionSampler, m_ReflectionCamera.targetTexture);
			}
		}

		public void OnEnable()
		{
			global::UnityEngine.Shader.EnableKeyword("WATER_REFLECTIVE");
			global::UnityEngine.Shader.DisableKeyword("WATER_SIMPLE");
		}

		public void OnDisable()
		{
			global::UnityEngine.Shader.EnableKeyword("WATER_SIMPLE");
			global::UnityEngine.Shader.DisableKeyword("WATER_REFLECTIVE");
		}

		private void RenderReflectionFor(global::UnityEngine.Camera cam, global::UnityEngine.Camera reflectCamera)
		{
			if (!reflectCamera || ((bool)m_SharedMaterial && !m_SharedMaterial.HasProperty(reflectionSampler)))
			{
				return;
			}
			reflectCamera.cullingMask = (int)reflectionMask & ~(1 << global::UnityEngine.LayerMask.NameToLayer("Water"));
			SaneCameraSettings(reflectCamera);
			reflectCamera.backgroundColor = clearColor;
			reflectCamera.clearFlags = (reflectSkybox ? global::UnityEngine.CameraClearFlags.Skybox : global::UnityEngine.CameraClearFlags.Color);
			if (reflectSkybox && (bool)cam.gameObject.GetComponent(typeof(global::UnityEngine.Skybox)))
			{
				global::UnityEngine.Skybox skybox = (global::UnityEngine.Skybox)reflectCamera.gameObject.GetComponent(typeof(global::UnityEngine.Skybox));
				if (!skybox)
				{
					skybox = (global::UnityEngine.Skybox)reflectCamera.gameObject.AddComponent(typeof(global::UnityEngine.Skybox));
				}
				skybox.material = ((global::UnityEngine.Skybox)cam.GetComponent(typeof(global::UnityEngine.Skybox))).material;
			}
			global::UnityEngine.GL.invertCulling = true;
			global::UnityEngine.Transform transform = base.transform;
			global::UnityEngine.Vector3 eulerAngles = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new global::UnityEngine.Vector3(0f - eulerAngles.x, eulerAngles.y, eulerAngles.z);
			reflectCamera.transform.position = cam.transform.position;
			global::UnityEngine.Vector3 position = transform.transform.position;
			position.y = transform.position.y;
			global::UnityEngine.Vector3 up = transform.transform.up;
			float w = 0f - global::UnityEngine.Vector3.Dot(up, position) - clipPlaneOffset;
			global::UnityEngine.Vector4 plane = new global::UnityEngine.Vector4(up.x, up.y, up.z, w);
			global::UnityEngine.Matrix4x4 zero = global::UnityEngine.Matrix4x4.zero;
			zero = CalculateReflectionMatrix(zero, plane);
			m_Oldpos = cam.transform.position;
			global::UnityEngine.Vector3 position2 = zero.MultiplyPoint(m_Oldpos);
			reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * zero;
			global::UnityEngine.Vector4 clipPlane = CameraSpacePlane(reflectCamera, position, up, 1f);
			global::UnityEngine.Matrix4x4 projectionMatrix = cam.projectionMatrix;
			projectionMatrix = CalculateObliqueMatrix(projectionMatrix, clipPlane);
			reflectCamera.projectionMatrix = projectionMatrix;
			reflectCamera.transform.position = position2;
			global::UnityEngine.Vector3 eulerAngles2 = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new global::UnityEngine.Vector3(0f - eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
			reflectCamera.Render();
			global::UnityEngine.GL.invertCulling = false;
		}

		private void SaneCameraSettings(global::UnityEngine.Camera helperCam)
		{
			helperCam.depthTextureMode = global::UnityEngine.DepthTextureMode.None;
			helperCam.backgroundColor = global::UnityEngine.Color.black;
			helperCam.clearFlags = global::UnityEngine.CameraClearFlags.Color;
			helperCam.renderingPath = global::UnityEngine.RenderingPath.Forward;
		}

		private static global::UnityEngine.Matrix4x4 CalculateObliqueMatrix(global::UnityEngine.Matrix4x4 projection, global::UnityEngine.Vector4 clipPlane)
		{
			global::UnityEngine.Vector4 b = projection.inverse * new global::UnityEngine.Vector4(Sgn(clipPlane.x), Sgn(clipPlane.y), 1f, 1f);
			global::UnityEngine.Vector4 vector = clipPlane * (2f / global::UnityEngine.Vector4.Dot(clipPlane, b));
			projection[2] = vector.x - projection[3];
			projection[6] = vector.y - projection[7];
			projection[10] = vector.z - projection[11];
			projection[14] = vector.w - projection[15];
			return projection;
		}

		private static global::UnityEngine.Matrix4x4 CalculateReflectionMatrix(global::UnityEngine.Matrix4x4 reflectionMat, global::UnityEngine.Vector4 plane)
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
			return reflectionMat;
		}

		private static float Sgn(float a)
		{
			if (a > 0f)
			{
				return 1f;
			}
			if (a < 0f)
			{
				return -1f;
			}
			return 0f;
		}

		private global::UnityEngine.Vector4 CameraSpacePlane(global::UnityEngine.Camera cam, global::UnityEngine.Vector3 pos, global::UnityEngine.Vector3 normal, float sideSign)
		{
			global::UnityEngine.Vector3 point = pos + normal * clipPlaneOffset;
			global::UnityEngine.Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			global::UnityEngine.Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
			global::UnityEngine.Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new global::UnityEngine.Vector4(rhs.x, rhs.y, rhs.z, 0f - global::UnityEngine.Vector3.Dot(lhs, rhs));
		}
	}
}
