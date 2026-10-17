public class RedirectToSettings : global::UnityEngine.MonoBehaviour
{
	public void openSettingsPage()
	{
		try
		{
			using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (global::UnityEngine.AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<global::UnityEngine.AndroidJavaObject>("currentActivity"))
				{
					using (global::UnityEngine.AndroidJavaObject androidJavaObject2 = new global::UnityEngine.AndroidJavaObject("android.content.Intent", "com.google.android.gms.settings.ADS_PRIVACY"))
					{
						androidJavaObject.Call("startActivityForResult", androidJavaObject2, 0);
					}
				}
			}
		}
		catch (global::System.Exception exception)
		{
			global::UnityEngine.Debug.LogException(exception);
		}
	}
}
