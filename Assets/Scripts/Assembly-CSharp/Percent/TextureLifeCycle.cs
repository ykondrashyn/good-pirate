namespace Percent
{
	public class TextureLifeCycle : global::UnityEngine.MonoBehaviour
	{
		public enum Status
		{
			INIT = 0,
			LOAD = 1,
			LOAD_ALL = 2
		}

		internal delegate void OnStateChange(global::Percent.TextureLifeCycle.Status state);

		public global::Percent.CrossPromotion crossPromotion;

		private global::Percent.TextureLifeCycle.Status state;

		internal global::Percent.TextureLifeCycle.OnStateChange onStateChange;

		private bool invokeFailAction;

		private int restJob;

		private global::UnityEngine.Events.UnityAction<bool> onAllTextureLoad;

		private global::Percent.TextureLifeCycle.Status State
		{
			set
			{
				state = value;
				if (onStateChange != null)
				{
					onStateChange(state);
				}
			}
		}

		private void Awake()
		{
			onAllTextureLoad = crossPromotion.onTextureLoad;
		}

		internal void load()
		{
			int num = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
			restJob = num;
			State = global::Percent.TextureLifeCycle.Status.LOAD;
		}

		internal void onEachTextureLoad(bool isSuccess)
		{
			if (isSuccess)
			{
				if (isAllJobDone(--restJob))
				{
					State = global::Percent.TextureLifeCycle.Status.LOAD_ALL;
					onAllTextureLoad(arg0: true);
				}
			}
			else if (!invokeFailAction)
			{
				onAllTextureLoad(arg0: false);
				invokeFailAction = true;
			}
		}

		private bool isAllJobDone(int restJob)
		{
			if (restJob.Equals(0))
			{
				return true;
			}
			return false;
		}
	}
}
