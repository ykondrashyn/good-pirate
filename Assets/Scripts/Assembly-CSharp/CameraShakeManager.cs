public class CameraShakeManager : global::UnityEngine.MonoBehaviour
{
	public static CameraShakeManager Instance;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform cameraLocalTrans;

	private void Awake()
	{
		Instance = this;
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	public void CameraShake(float duration = 0.2f, float strength = 0.3f, bool fadeOut = true)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(cameraLocalTrans);
		cameraLocalTrans.localPosition = global::UnityEngine.Vector3.zero;
		cameraLocalTrans.localRotation = global::UnityEngine.Quaternion.identity;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOShakePosition(cameraLocalTrans, duration, strength, 20, 360f, snapping: false, fadeOut), delegate
		{
			cameraLocalTrans.localPosition = global::UnityEngine.Vector3.zero;
		});
	}

	public void CameraShakeRotate(float duration = 0.2f, float strength = 0.3f, bool fadeOut = true)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(cameraLocalTrans);
		cameraLocalTrans.localPosition = global::UnityEngine.Vector3.zero;
		cameraLocalTrans.localRotation = global::UnityEngine.Quaternion.identity;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOShakeRotation(cameraLocalTrans, duration, strength, 20, 360f, fadeOut), delegate
		{
			cameraLocalTrans.localRotation = global::UnityEngine.Quaternion.identity;
		});
	}
}
