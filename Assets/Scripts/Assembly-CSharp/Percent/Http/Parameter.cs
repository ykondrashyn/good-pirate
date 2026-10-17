namespace Percent.Http
{
	public class Parameter
	{
		private string url;

		private bool isFirstParameter = true;

		internal Parameter(string prefix)
		{
			url = prefix;
		}

		internal void addParamater(string key, string[] paramValues)
		{
			if (isFirstParameter)
			{
				url = url + global::Percent.Http.PercentHttpConfig.QUERY_STRING + key + global::Percent.Http.PercentHttpConfig.EQUAL + getSetOf(paramValues);
				isFirstParameter = false;
			}
			else
			{
				url = url + global::Percent.Http.PercentHttpConfig.AND + key + global::Percent.Http.PercentHttpConfig.EQUAL + getSetOf(paramValues);
			}
		}

		private string getSetOf(string[] paramValues)
		{
			string text = paramValues[0];
			for (int i = 1; i < paramValues.Length; i++)
			{
				text = text + "," + paramValues[i];
			}
			return text;
		}

		internal void addParamater(string key, string paramValue)
		{
			if (isFirstParameter)
			{
				url = url + global::Percent.Http.PercentHttpConfig.QUERY_STRING + key + global::Percent.Http.PercentHttpConfig.EQUAL + paramValue;
				isFirstParameter = false;
			}
			else
			{
				url = url + global::Percent.Http.PercentHttpConfig.AND + key + global::Percent.Http.PercentHttpConfig.EQUAL + paramValue;
			}
		}

		internal void addParamater(string key, int paramValue)
		{
			addParamater(key, paramValue.ToString());
		}

		internal string getUrl()
		{
			return url;
		}
	}
}
