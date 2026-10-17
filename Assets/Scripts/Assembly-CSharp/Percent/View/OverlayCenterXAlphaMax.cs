namespace Percent.View
{
	public class OverlayCenterXAlphaMax : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.UI.RawImage[] target;

		public float alphaMaxDistance = 300f;

		public float alphaZeroDistance = 600f;

		public float fixedCenterX;

		private global::UnityEngine.RectTransform thisTrans;

		private float centerX;

		private float distanceFromCenter;

		private void Awake()
		{
			thisTrans = GetComponent<global::UnityEngine.RectTransform>();
		}

		private void Start()
		{
			centerX = (float)(global::UnityEngine.Screen.width / 2) + fixedCenterX;
		}

		internal void startAnimation()
		{
			StartCoroutine(animate());
		}

		private global::System.Collections.IEnumerator animate()
		{
			while (true)
			{
				distanceFromCenter = global::UnityEngine.Mathf.Abs(centerX - thisTrans.position.x);
				float a = (isAlphaZeroDistance() ? 0f : ((!isAlphaMaxDistance()) ? (1f - (distanceFromCenter - alphaMaxDistance) / (alphaZeroDistance - alphaMaxDistance)) : 1f));
				for (int i = 0; i < target.Length; i++)
				{
					target[i].color = new global::UnityEngine.Color(target[i].color.r, target[i].color.g, target[i].color.b, a);
				}
				yield return null;
			}
		}

		private bool isAlphaZeroDistance()
		{
			if (distanceFromCenter >= alphaZeroDistance)
			{
				return true;
			}
			return false;
		}

		private bool isAlphaMaxDistance()
		{
			if (distanceFromCenter <= alphaMaxDistance)
			{
				return true;
			}
			return false;
		}
	}
}
