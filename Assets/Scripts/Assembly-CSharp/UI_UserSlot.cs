public class UI_UserSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject userObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject meObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject waitingObj;

	private void AllDisable()
	{
		userObj.SetActive(value: false);
		meObj.SetActive(value: false);
		waitingObj.SetActive(value: false);
	}

	public void SetUser()
	{
		AllDisable();
		userObj.SetActive(value: true);
	}

	public void SetMe()
	{
		AllDisable();
		meObj.SetActive(value: true);
	}

	public void SetWaiting()
	{
		AllDisable();
		waitingObj.SetActive(value: true);
	}
}
