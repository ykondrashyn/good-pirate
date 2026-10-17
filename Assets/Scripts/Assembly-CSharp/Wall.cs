public class Wall : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTransform;

	public void SetSize(int size, bool isHaveGate = true)
	{
		if (isHaveGate)
		{
			modelTransform.localPosition = new global::UnityEngine.Vector3(-20f, 0f, 0f);
			modelTransform.localScale = new global::UnityEngine.Vector3((float)(size / 2 - 20) + 1.7f, 1f, 1f);
		}
		else
		{
			modelTransform.localPosition = global::UnityEngine.Vector3.zero;
			modelTransform.localScale = new global::UnityEngine.Vector3((float)(size / 2) + 1.7f, 1f, 1f);
		}
	}
}
