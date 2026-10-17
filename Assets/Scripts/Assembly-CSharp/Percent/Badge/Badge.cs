namespace Percent.Badge
{
	public class Badge : global::Percent.Http.HttpsClient
	{
		public bool refreshOnEnable = true;

		private global::Percent.Badge.BadgeData pickedBadge;

		private global::UnityEngine.UI.RawImage targetRenderer;

		private global::Percent.ImageTool imageTool;

		private global::Percent.Tween.ColorTween showTween;

		private global::Percent.Event.ClickTracker clickTracker;

		private static readonly int VALUE_MAX_LOAD_TEXTURE_RETRY = 20;

		private bool isLocalCache;

		private void Awake()
		{
			targetRenderer = GetComponent<global::UnityEngine.UI.RawImage>();
			imageTool = new global::Percent.ImageTool();
			showTween = GetComponent<global::Percent.Tween.ColorTween>();
			clickTracker = base.gameObject.AddComponent<global::Percent.Event.ClickTracker>();
		}

		private void OnEnable()
		{
			if (refreshOnEnable)
			{
				StartCoroutine(pollingData());
			}
		}

		private void OnDisable()
		{
			showTween.resetToBegining();
		}

		private global::System.Collections.IEnumerator pollingData()
		{
			int retryCount = 0;
			do
			{
				retryCount++;
				yield return null;
			}
			while (!isLoadReady(retryCount));
			ifBadgePoolIsNullThenLoadOldPool();
			if (!global::Percent.PromotionData.isBadgePoolNull())
			{
				pickRandomBadgeUp();
				loadTexture();
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private bool isLoadReady(int retryCount)
		{
			if (retryCount >= VALUE_MAX_LOAD_TEXTURE_RETRY || !global::Percent.PromotionData.isBadgePoolNull())
			{
				return true;
			}
			return false;
		}

		private void ifBadgePoolIsNullThenLoadOldPool()
		{
			if (global::Percent.PromotionData.isBadgePoolNull())
			{
				loadBadgePoolFromLocalCache();
			}
		}

		private void loadBadgePoolFromLocalCache()
		{
			global::Percent.PromotionData.badgePool = global::Percent.Badge.BadgePoolCacher.load();
		}

		private void pickRandomBadgeUp()
		{
			int max = global::Percent.PromotionData.badgePool.Length;
			int num = global::UnityEngine.Random.Range(0, max);
			pickedBadge.gameId = global::Percent.PromotionData.badgePool[num].gameId;
			pickedBadge.iconUrl = global::Percent.PromotionData.badgePool[num].iconUrl;
			pickedBadge.storeUrl = global::Percent.PromotionData.badgePool[num].storeUrl;
		}

		private void loadTexture()
		{
			if (!imageTool.isCacheExist(global::Percent.Http.HttpsClient.resourceIdOf(pickedBadge.iconUrl)))
			{
				sendTextureRequest(pickedBadge.iconUrl);
			}
			else
			{
				tryToLoadCacheIfFailSendRequest();
			}
		}

		private void tryToLoadCacheIfFailSendRequest()
		{
			string resourceId = global::Percent.Http.HttpsClient.resourceIdOf(pickedBadge.iconUrl);
			global::UnityEngine.Texture2D texture2D = imageTool.loadTextureFromCache(resourceId);
			if (texture2D == null)
			{
				global::Percent.Logger.error("Load badge cache texture from local is FAIL. Try to download via https : " + pickedBadge.iconUrl);
				sendTextureRequest(pickedBadge.iconUrl);
			}
			else
			{
				onTextureResponseSuccess(texture2D);
				isLocalCache = true;
			}
		}

		internal override void onTextureResponseSuccess(global::UnityEngine.Texture2D texture)
		{
			targetRenderer.texture = texture;
			targetRenderer.color = global::UnityEngine.Color.white;
			showTween.play();
			saveCacheTextureToLocal(texture);
			if (!isLocalCache)
			{
				global::Percent.Badge.BadgePoolCacher.save();
			}
			base.onTextureResponseSuccess(texture);
		}

		private void saveCacheTextureToLocal(global::UnityEngine.Texture2D texture)
		{
			string resourceId = global::Percent.Http.HttpsClient.resourceIdOf(pickedBadge.iconUrl);
			if (!imageTool.isCacheExist(resourceId))
			{
				imageTool.saveTexture(resourceId, texture);
			}
		}

		public void onClick()
		{
			global::Percent.Util.goMarket(pickedBadge.storeUrl);
			clickTracker.trigger(global::Percent.PromotionType.BADGE, pickedBadge.gameId);
		}
	}
}
