namespace Percent
{
	public class Deleter : global::UnityEngine.MonoBehaviour
	{
		public bool playerPref;

		public bool imageCache;

		private void Awake()
		{
			if (playerPref)
			{
				global::UnityEngine.PlayerPrefs.DeleteAll();
			}
			if (imageCache)
			{
				new global::Percent.ImageTool().deleteAllCache();
			}
		}
	}
}
