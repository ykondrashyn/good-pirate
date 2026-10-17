namespace Percent.Http
{
	public class TextureLoader : global::Percent.Http.HttpsClient
	{
		private global::UnityEngine.UI.RawImage targetRenderer;

		private global::Percent.ImageTool imageTool;

		private string imageUrl;

		private void Awake()
		{
			targetRenderer = GetComponent<global::UnityEngine.UI.RawImage>();
			imageTool = new global::Percent.ImageTool();
		}

		internal void render(string imageUrl, global::UnityEngine.Events.UnityAction<bool> onResponse)
		{
			this.imageUrl = imageUrl;
			onTextureResponse = onResponse;
			if (!imageTool.isCacheExist(global::Percent.Http.HttpsClient.resourceIdOf(imageUrl)))
			{
				sendTextureRequest(imageUrl);
			}
			else
			{
				tryToLoadCacheIfFailSendRequest(imageUrl);
			}
		}

		private void tryToLoadCacheIfFailSendRequest(string imageUrl)
		{
			global::UnityEngine.Texture2D texture2D = imageTool.loadTextureFromCache(global::Percent.Http.HttpsClient.resourceIdOf(imageUrl));
			if (texture2D == null)
			{
				global::Percent.Logger.error("Load Cache texture from local is FAIL. Try to download via https : " + imageUrl);
				sendTextureRequest(imageUrl);
			}
			else
			{
				onTextureResponseSuccess(texture2D);
			}
		}

		internal override void onTextureResponseSuccess(global::UnityEngine.Texture2D texture)
		{
			targetRenderer.texture = texture;
			saveCacheTextureToLocal(texture);
			base.onTextureResponseSuccess(texture);
		}

		private void saveCacheTextureToLocal(global::UnityEngine.Texture2D texture)
		{
			string resourceId = global::Percent.Http.HttpsClient.resourceIdOf(imageUrl);
			if (!imageTool.isCacheExist(resourceId))
			{
				imageTool.saveTexture(resourceId, texture);
			}
		}
	}
}
