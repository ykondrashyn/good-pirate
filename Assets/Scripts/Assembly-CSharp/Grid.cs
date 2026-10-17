public class Grid
{
	public global::System.Collections.Generic.List<global::UnityEngine.GameObject> haveObjList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	public void AllActive(bool active)
	{
		foreach (global::UnityEngine.GameObject haveObj in haveObjList)
		{
			if (haveObj != null && haveObj.activeSelf != active)
			{
				haveObj.SetActive(active);
			}
		}
	}

	public void AddObject(global::UnityEngine.GameObject obj)
	{
		if (!haveObjList.Contains(obj))
		{
			haveObjList.Add(obj);
		}
	}
}
