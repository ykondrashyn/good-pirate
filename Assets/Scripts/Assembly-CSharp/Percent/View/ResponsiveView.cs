namespace Percent.View
{
	public class ResponsiveView : global::Percent.View.View
	{
		public global::UnityEngine.Vector3 verticalInitPos;

		public global::UnityEngine.Vector2 verticalInitSize;

		public global::UnityEngine.Vector3 horizontalInitPos;

		public global::UnityEngine.Vector2 horizontalInitSize;

		public bool respondOnEnable;

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
			respond();
		}

		internal virtual void respond()
		{
			switch (global::Percent.Util.getScreenOrientation())
			{
			case global::Percent.View.ScreenOrientation.VERTICAL:
				thisTrans.localPosition = verticalInitPos;
				thisTrans.sizeDelta = verticalInitSize;
				break;
			case global::Percent.View.ScreenOrientation.HORIZONTAL:
				thisTrans.localPosition = horizontalInitPos;
				thisTrans.sizeDelta = horizontalInitSize;
				break;
			}
		}

		protected override void OnEnable()
		{
			if (respondOnEnable)
			{
				respond();
			}
		}
	}
}
