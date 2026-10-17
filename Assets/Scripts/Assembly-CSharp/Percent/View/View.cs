namespace Percent.View
{
	public class View : global::UnityEngine.MonoBehaviour
	{
		protected global::UnityEngine.UI.RawImage render;

		protected global::Percent.View.ViewLifeCycle viewLifeCycle;

		protected global::UnityEngine.RectTransform thisTrans;

		protected global::UnityEngine.Color NOT_VISIBLE = new global::UnityEngine.Color(1f, 1f, 1f, 0f);

		protected virtual void Awake()
		{
			render = GetComponent<global::UnityEngine.UI.RawImage>();
			viewLifeCycle = global::UnityEngine.GameObject.Find("ViewLifeCycle").GetComponent<global::Percent.View.ViewLifeCycle>();
			global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
			obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
			thisTrans = GetComponent<global::UnityEngine.RectTransform>();
		}

		protected virtual void Start()
		{
			render.color = NOT_VISIBLE;
		}

		protected virtual void OnEnable()
		{
		}

		internal virtual void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
		}
	}
}
