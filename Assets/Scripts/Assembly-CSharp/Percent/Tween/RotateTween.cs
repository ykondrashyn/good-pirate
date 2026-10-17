namespace Percent.Tween
{
	public class RotateTween : global::UnityEngine.MonoBehaviour
	{
		public float rotationSpeed;

		private global::UnityEngine.RectTransform thisTrans;

		private global::UnityEngine.UI.Image image;

		private global::System.Collections.IEnumerator rotateCoroutine;

		private void Awake()
		{
			thisTrans = GetComponent<global::UnityEngine.RectTransform>();
			image = GetComponent<global::UnityEngine.UI.Image>();
			rotateCoroutine = rotate();
		}

		public void enableRotation(bool isEnable)
		{
			if (isEnable)
			{
				StartCoroutine(rotateCoroutine);
				image.color = global::UnityEngine.Color.white;
			}
			else
			{
				StopCoroutine(rotateCoroutine);
				image.color = global::UnityEngine.Color.clear;
			}
		}

		private global::System.Collections.IEnumerator rotate()
		{
			while (true)
			{
				thisTrans.Rotate(0f, 0f, rotationSpeed);
				yield return null;
			}
		}
	}
}
