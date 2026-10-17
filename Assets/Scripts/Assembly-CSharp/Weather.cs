[global::System.Serializable]
public struct Weather
{
	public string name;

	public EWeatherType weatherType;

	public global::UnityEngine.Color cameraColor;

	public global::UnityEngine.Color lightColor;

	public global::UnityEngine.Events.UnityEvent startCallBack;

	public global::UnityEngine.Events.UnityEvent endCallBack;
}
