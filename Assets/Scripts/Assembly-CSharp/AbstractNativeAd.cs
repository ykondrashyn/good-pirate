public abstract class AbstractNativeAd : global::UnityEngine.MonoBehaviour
{
	public struct Data
	{
		public global::System.Uri MainImageUrl;

		public global::System.Uri IconImageUrl;

		public global::System.Uri ClickDestinationUrl;

		public string CallToAction;

		public string Title;

		public string Text;

		public double StarRating;

		public global::System.Uri PrivacyInformationIconClickThroughUrl;

		public global::System.Uri PrivacyInformationIconImageUrl;

		public static global::System.Uri ToUri(object value)
		{
			global::System.Uri uri = value as global::System.Uri;
			if (uri != null)
			{
				return uri;
			}
			string text = value as string;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (global::System.Uri.IsWellFormedUriString(text, global::System.UriKind.Absolute))
			{
				return new global::System.Uri(text, global::System.UriKind.Absolute);
			}
			global::UnityEngine.Debug.LogError("Invalid URL: " + text);
			return null;
		}

		public static AbstractNativeAd.Data FromJson(string json)
		{
			global::System.Collections.Generic.Dictionary<string, object> dictionary = (global::MoPubInternal.ThirdParty.MiniJSON.Json.Deserialize(json) as global::System.Collections.Generic.Dictionary<string, object>) ?? new global::System.Collections.Generic.Dictionary<string, object>();
			object value;
			return new AbstractNativeAd.Data
			{
				MainImageUrl = (dictionary.TryGetValue("mainImageUrl", out value) ? ToUri(value) : null),
				IconImageUrl = (dictionary.TryGetValue("iconImageUrl", out value) ? ToUri(value) : null),
				ClickDestinationUrl = (dictionary.TryGetValue("clickDestinationUrl", out value) ? ToUri(value) : null),
				CallToAction = (dictionary.TryGetValue("callToAction", out value) ? (value as string) : string.Empty),
				Title = (dictionary.TryGetValue("title", out value) ? (value as string) : string.Empty),
				Text = (dictionary.TryGetValue("text", out value) ? (value as string) : string.Empty),
				StarRating = (dictionary.TryGetValue("starRating", out value) ? ((double)value) : 0.0),
				PrivacyInformationIconClickThroughUrl = (dictionary.TryGetValue("privacyInformationIconClickThroughUrl", out value) ? ToUri(value) : null),
				PrivacyInformationIconImageUrl = (dictionary.TryGetValue("privacyInformationIconImageUrl", out value) ? ToUri(value) : null)
			};
		}

		public override string ToString()
		{
			return $"mainImageUrl: {MainImageUrl}\niconImageUrl: {IconImageUrl}\nclickDestinationUrl: {ClickDestinationUrl}\ncallToAction: {CallToAction}\ntitle: {Title}\ntext: {Text}\nstarRating: {StarRating}\nprivacyInformationIconClickThroughUrl: {PrivacyInformationIconClickThroughUrl}\nprivacyInformationIconImageUrl: {PrivacyInformationIconImageUrl}";
		}
	}

	public string AdUnitId;

	[global::UnityEngine.Header("Text")]
	public global::UnityEngine.UI.Text Title;

	public global::UnityEngine.UI.Text Text;

	public global::UnityEngine.UI.Text CallToAction;

	[global::UnityEngine.Header("Images")]
	public global::UnityEngine.Renderer MainImage;

	public global::UnityEngine.Renderer IconImage;

	public global::UnityEngine.Renderer PrivacyInformationIconImage;
}
