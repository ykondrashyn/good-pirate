// Preserve scene identity and recovered callers without collecting any data.
public class GoogleAnalyticsV3 : UnityEngine.MonoBehaviour
{
    public static GoogleAnalyticsV3 instance;
    public string androidTrackingCode, IOSTrackingCode, otherTrackingCode;
    public string productName, bundleIdentifier, bundleVersion;
    public int dispatchPeriod, sampleFrequency, logLevel, sessionTimeout;
    public bool anonymizeIP, UncaughtExceptionReporting, sendLaunchEvent, dryRun;
    private void Awake() { instance = this; }
    public void LogEvent(string category, string action, string label, long value) { }
    public void LogScreen(string screen) { }
}
