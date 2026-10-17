public abstract class PackageConfig
{
	public enum Platform
	{
		ANDROID = 0,
		IOS = 1
	}

	public abstract string Name { get; }

	public abstract string Version { get; }

	public abstract global::System.Collections.Generic.Dictionary<PackageConfig.Platform, string> NetworkSdkVersions { get; }

	public abstract global::System.Collections.Generic.Dictionary<PackageConfig.Platform, string> AdapterClassNames { get; }

	public string NetworkSdkVersion => NetworkSdkVersions[PackageConfig.Platform.ANDROID];

	public string AdapterClassName => AdapterClassNames[PackageConfig.Platform.ANDROID];
}
