namespace Percent
{
	public class ImageTool
	{
		internal static readonly string CACHE_PATH = global::UnityEngine.Application.persistentDataPath + "/cache";

		private readonly string SLASH = "/";

		private readonly string EX_PNG = ".png";

		internal ImageTool()
		{
			if (!global::System.IO.Directory.Exists(CACHE_PATH))
			{
				global::System.IO.Directory.CreateDirectory(CACHE_PATH);
			}
		}

		internal bool isCacheExist(string resourceId)
		{
			string path = resolvePath(resourceId);
			if (isFileExist(path))
			{
				return true;
			}
			return false;
		}

		private bool isFileExist(string path)
		{
			if (new global::System.IO.FileInfo(path).Exists)
			{
				return true;
			}
			return false;
		}

		internal void saveTexture(string resourceId, global::UnityEngine.Texture2D texture)
		{
			try
			{
				byte[] bytes = global::UnityEngine.ImageConversion.EncodeToPNG(texture);
				global::System.IO.File.WriteAllBytes(resolvePath(resourceId), bytes);
			}
			catch
			{
				global::Percent.Logger.error("Save cache image FAIL.");
			}
		}

		private string resolvePath(string fileName)
		{
			return CACHE_PATH + SLASH + fileName + EX_PNG;
		}

		internal global::UnityEngine.Texture2D loadTextureFromCache(string resourceId)
		{
			string text = resolvePath(resourceId);
			if (!isFileExist(text))
			{
				global::Percent.Logger.error("There is no cached file.");
				return null;
			}
			return loadPNG(text);
		}

		internal global::UnityEngine.Texture2D loadPNG(string filePath)
		{
			global::UnityEngine.Texture2D texture2D = null;
			if (isFileExist(filePath))
			{
				try
				{
					byte[] data = global::System.IO.File.ReadAllBytes(filePath);
					texture2D = new global::UnityEngine.Texture2D(2, 2, global::UnityEngine.TextureFormat.RGBA32, mipChain: false);
					global::UnityEngine.ImageConversion.LoadImage(texture2D, data);
				}
				catch
				{
					global::Percent.Logger.error("Cached file load FAIL");
				}
			}
			return texture2D;
		}

		internal void deleteAllCache()
		{
			if (global::System.IO.Directory.Exists(CACHE_PATH))
			{
				global::System.IO.Directory.Delete(CACHE_PATH, recursive: true);
			}
		}
	}
}
