namespace UnityStandardAssets.Water
{
	[global::UnityEngine.ExecuteInEditMode]
	public class WaterTile : global::UnityEngine.MonoBehaviour
	{
		public global::UnityStandardAssets.Water.PlanarReflection reflection;

		public global::UnityStandardAssets.Water.WaterBase waterBase;

		public void Start()
		{
			AcquireComponents();
		}

		private void AcquireComponents()
		{
			if (!reflection)
			{
				if ((bool)base.transform.parent)
				{
					reflection = base.transform.parent.GetComponent<global::UnityStandardAssets.Water.PlanarReflection>();
				}
				else
				{
					reflection = base.transform.GetComponent<global::UnityStandardAssets.Water.PlanarReflection>();
				}
			}
			if (!waterBase)
			{
				if ((bool)base.transform.parent)
				{
					waterBase = base.transform.parent.GetComponent<global::UnityStandardAssets.Water.WaterBase>();
				}
				else
				{
					waterBase = base.transform.GetComponent<global::UnityStandardAssets.Water.WaterBase>();
				}
			}
		}

		public void OnWillRenderObject()
		{
			if ((bool)reflection)
			{
				reflection.WaterTileBeingRendered(base.transform, global::UnityEngine.Camera.current);
			}
			if ((bool)waterBase)
			{
				waterBase.WaterTileBeingRendered(base.transform, global::UnityEngine.Camera.current);
			}
		}
	}
}
