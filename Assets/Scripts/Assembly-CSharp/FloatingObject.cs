public class FloatingObject : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	private void Awake()
	{
		modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(global::UnityEngine.Random.Range(-5f, 5f), global::UnityEngine.Random.Range(0f, 360f), global::UnityEngine.Random.Range(-5f, 5f));
	}

	private void OnEnable()
	{
		global::DG.Tweening.TweenSettingsExtensions.SetLoops(global::DG.Tweening.ShortcutExtensions.DORotate(base.transform, new global::UnityEngine.Vector3(0f, 360f, 0f), 30f, global::DG.Tweening.RotateMode.FastBeyond360), -1);
	}

	private void OnDisable()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(base.transform);
	}
}
