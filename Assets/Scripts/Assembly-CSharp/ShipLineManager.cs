public class ShipLineManager : global::UnityEngine.MonoBehaviour
{
	public static ShipLineManager Instance;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<LineData> lineList;

	private global::System.Collections.Generic.Dictionary<ELine, ShipLineList> lineDic = new global::System.Collections.Generic.Dictionary<ELine, ShipLineList>();

	private void Awake()
	{
		Instance = this;
		foreach (LineData line in lineList)
		{
			ShipLineList component = global::UnityEngine.Object.Instantiate(line.src, base.transform).GetComponent<ShipLineList>();
			lineDic.Add(line.id, component);
		}
	}

	public global::System.Collections.Generic.List<global::UnityEngine.Transform> GetLineData(ELine lineType)
	{
		if (lineDic.ContainsKey(lineType))
		{
			return lineDic[lineType].lineList;
		}
		return null;
	}

	public global::System.Collections.Generic.List<global::UnityEngine.Transform> GetRandomLineData()
	{
		return lineDic[lineList[0].id].lineList;
	}
}
