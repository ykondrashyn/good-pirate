namespace Percent.Http
{
	public class HttpsClient : global::UnityEngine.MonoBehaviour
	{
		internal global::UnityEngine.Events.UnityAction<bool> onGETResponse;

		internal global::UnityEngine.Events.UnityAction<bool> onTextureResponse;

		private string url;

		internal void sendTextureRequest(string url)
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("http", "percent-texture-request");
			onTextureResponseFail();
		}

		internal global::System.Collections.IEnumerator sendTextureRequestCoroutine(string url)
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("http", "percent-texture-request");
			onTextureResponseFail();
			yield break;
		}

		internal virtual void onTextureResponseSuccess(global::UnityEngine.Texture2D texture)
		{
			if (onTextureResponse != null)
			{
				onTextureResponse(arg0: true);
			}
		}

		internal virtual void onTextureResponseFail()
		{
			if (onTextureResponse != null)
			{
				onTextureResponse(arg0: false);
			}
		}

		internal void sendGETRequest(string url, bool isResponseJson = false)
		{
			this.url = url;
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("http", "percent-get-request");
			onGETResponseFail();
		}

		internal global::System.Collections.IEnumerator sendGETRequestCoroutine(bool isResponseJson = false)
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("http", "percent-get-request");
			onGETResponseFail();
			yield break;
		}

		internal virtual void onGETResponseSuccess()
		{
			if (onGETResponse != null)
			{
				onGETResponse(arg0: true);
			}
		}

		internal virtual void onGETResponseSuccess(global::Boomlagoon.JSON.JSONObject json)
		{
			if (onGETResponse != null)
			{
				onGETResponse(arg0: true);
			}
		}

		internal virtual void onGETResponseFail()
		{
			if (onGETResponse != null)
			{
				onGETResponse(arg0: false);
			}
		}

		internal static string resolve(string url)
		{
			return global::Percent.Http.PercentHttpConfig.HOST + url;
		}

		internal static void addResource<T>(ref string url, T resource)
		{
			url = url + global::Percent.Http.PercentHttpConfig.SLASH + resource;
		}

		internal static bool isRequestError(global::UnityEngine.WWW www)
		{
			if (www.error == null)
			{
				return false;
			}
			return true;
		}

		internal static bool isRequestSuccess(global::UnityEngine.WWW www)
		{
			if (isStatus200OK(www))
			{
				return true;
			}
			return false;
		}

		private static bool isStatus200OK(global::UnityEngine.WWW www)
		{
			if (getStatusCode(www).Contains("200"))
			{
				return true;
			}
			return false;
		}

		internal static bool isStatus301MovedPermanently(global::UnityEngine.WWW www)
		{
			if (getStatusCode(www).Contains("301"))
			{
				return true;
			}
			return false;
		}

		internal static bool isStatus302MovedTemporarily(global::UnityEngine.WWW www)
		{
			if (getStatusCode(www).Contains("302"))
			{
				return true;
			}
			return false;
		}

		private static string getStatusCode(global::UnityEngine.WWW www)
		{
			www.responseHeaders.TryGetValue(global::Percent.Http.PercentHttpConfig.RES_STATUS, out var value);
			return value;
		}

		private static string getLocation(global::UnityEngine.WWW www)
		{
			www.responseHeaders.TryGetValue(global::Percent.Http.PercentHttpConfig.RES_LOCATION, out var value);
			return value;
		}

		internal static string resourceIdOf(string url)
		{
			int num = url.LastIndexOf(global::Percent.Http.PercentHttpConfig.SLASH);
			return url.Substring(num + 1);
		}

		internal static string finalRedirectionUrlOf(string weirdUrl)
		{
			int num = weirdUrl.LastIndexOf(global::Percent.Http.PercentHttpConfig.COMMA);
			if (!num.Equals(-1))
			{
				return weirdUrl.Substring(num + 1);
			}
			return weirdUrl;
		}
	}
}
