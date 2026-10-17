namespace Percent
{
	public class TextTool
	{
		internal static readonly string CACHE_PATH = global::UnityEngine.Application.persistentDataPath + "/cache";

		private readonly string SLASH = "/";

		private readonly string EX_PRIVACY_POLICY = ".text";

		internal TextTool()
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

		internal void saveText(string resourceId, string text)
		{
			try
			{
				global::System.IO.StreamWriter streamWriter = global::System.IO.File.CreateText(resolvePath(resourceId));
				streamWriter.Write(text);
				streamWriter.Close();
			}
			catch
			{
				global::Percent.Logger.error("Save cache text FAIL.");
			}
		}

		private string resolvePath(string fileName)
		{
			return CACHE_PATH + SLASH + fileName + EX_PRIVACY_POLICY;
		}

		internal string loadTextFromCache(string resourceId)
		{
			string text = resolvePath(resourceId);
			if (!isFileExist(text))
			{
				global::Percent.Logger.error("There is no cached file.");
				return string.Empty;
			}
			return loadText(text);
		}

		internal string loadText(string filePath)
		{
			if (isFileExist(filePath))
			{
				try
				{
					return new global::System.IO.StreamReader(filePath).ReadToEnd();
				}
				catch
				{
					global::Percent.Logger.error("Cached file load FAIL");
				}
			}
			return string.Empty;
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
