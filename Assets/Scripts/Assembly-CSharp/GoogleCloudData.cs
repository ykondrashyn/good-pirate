[global::System.Serializable]
public class GoogleCloudData
{
	public global::System.Collections.Generic.Dictionary<string, string> stringDic;

	public global::System.Collections.Generic.Dictionary<string, int> intDic;

	public global::System.Collections.Generic.Dictionary<string, bool> boolDic;

	public void Clear()
	{
		if (stringDic != null)
		{
			stringDic.Clear();
		}
		if (intDic != null)
		{
			intDic.Clear();
		}
		if (boolDic != null)
		{
			boolDic.Clear();
		}
	}

	public GoogleCloudData()
	{
		stringDic = new global::System.Collections.Generic.Dictionary<string, string>();
		intDic = new global::System.Collections.Generic.Dictionary<string, int>();
		boolDic = new global::System.Collections.Generic.Dictionary<string, bool>();
	}
}
