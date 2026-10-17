public class TreeEventerListener : global::UnityEngine.MonoBehaviour
{
	private Tree myTree;

	private void Awake()
	{
		myTree = GetComponentInParent<Tree>();
	}

	public void CircleWarnOn()
	{
		myTree.circleWarnObj.SetActive(value: true);
	}

	public void CircleHitOn()
	{
		myTree.circleWarnObj.SetActive(value: false);
		myTree.circleHitObj.gameObject.SetActive(value: true);
		global::UnityEngine.Vector3 position = myTree.circleHitObj.transform.position;
		position.y = 0.1f;
		global::UnityEngine.GameObject obj = null;
		ObjectPoolManager.TryGetNextObject("WaterSplash03", position, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f), out obj);
		obj.transform.localScale = global::UnityEngine.Vector3.one * 5f;
	}

	public void CircleOff()
	{
		myTree.circleHitObj.gameObject.SetActive(value: false);
	}

	public void QuadWarnOn()
	{
		myTree.quadWarnObj.SetActive(value: true);
	}

	public void QuadHitOn()
	{
		myTree.quadWarnObj.SetActive(value: false);
		myTree.quadHitObj.gameObject.SetActive(value: true);
		global::UnityEngine.Vector3 position = myTree.quadHitObj.transform.position;
		position.y = 0.1f;
		global::UnityEngine.GameObject obj = null;
		ObjectPoolManager.TryGetNextObject("WaterSplash03", position, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f), out obj);
		obj.transform.localScale = global::UnityEngine.Vector3.one * 5f;
	}

	public void QuadOff()
	{
		myTree.quadHitObj.gameObject.SetActive(value: false);
	}

	public void SawWarnOn()
	{
		myTree.sawWarnObj.SetActive(value: true);
	}

	public void SawHitOn()
	{
		myTree.sawWarnObj.SetActive(value: false);
		myTree.sawHitObj.gameObject.SetActive(value: true);
	}

	public void SawOff()
	{
		myTree.sawHitObj.gameObject.SetActive(value: false);
	}
}
