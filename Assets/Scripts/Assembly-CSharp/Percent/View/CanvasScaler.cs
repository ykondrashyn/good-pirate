namespace Percent.View
{
	public class CanvasScaler : global::UnityEngine.MonoBehaviour
	{
		private void Awake()
		{
			setCanvasScalerByMode();
		}

		private void setCanvasScalerByMode()
		{
			float num = 1242f;
			float num2 = 2208f;
			global::UnityEngine.UI.CanvasScaler component = GetComponent<global::UnityEngine.UI.CanvasScaler>();
			if (global::Percent.Util.getScreenOrientation().Equals(global::Percent.View.ScreenOrientation.VERTICAL))
			{
				component.referenceResolution = new global::UnityEngine.Vector2(num, num2);
			}
			else
			{
				component.referenceResolution = new global::UnityEngine.Vector2(num2, num);
			}
		}
	}
}
