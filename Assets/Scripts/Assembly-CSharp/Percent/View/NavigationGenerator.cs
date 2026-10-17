namespace Percent.View
{
	public class NavigationGenerator : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.GameObject prefabDot;

		private global::Percent.TextureLifeCycle textureLifeCycle;

		private void Awake()
		{
			textureLifeCycle = global::UnityEngine.GameObject.Find("TextureLifeCycle").GetComponent<global::Percent.TextureLifeCycle>();
			global::Percent.TextureLifeCycle obj = textureLifeCycle;
			obj.onStateChange = (global::Percent.TextureLifeCycle.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.TextureLifeCycle.OnStateChange(onTextureStateChange));
		}

		internal void onTextureStateChange(global::Percent.TextureLifeCycle.Status state)
		{
			if (state == global::Percent.TextureLifeCycle.Status.LOAD)
			{
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
				{
					global::Percent.TextureLifeCycle obj = textureLifeCycle;
					obj.onStateChange = (global::Percent.TextureLifeCycle.OnStateChange)global::System.Delegate.Remove(obj.onStateChange, new global::Percent.TextureLifeCycle.OnStateChange(onTextureStateChange));
				}
				else if (global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
				{
					generate();
				}
			}
		}

		private void generate()
		{
			int num = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
			for (int i = 0; i < num; i++)
			{
				global::UnityEngine.Object.Instantiate(prefabDot, global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.identity);
			}
		}
	}
}
