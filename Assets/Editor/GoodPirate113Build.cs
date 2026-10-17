using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace GoodPirate113.Editor
{
    public static class GoodPirate113Build
    {
        public const string AndroidPackage =
            "com.reconstruction.goodpirate113";
        public const string ProductName =
            "Good Pirate 1.13 Reconstruction";
        public const string VersionName = "1.13-r1";
        public const int VersionCode = 1130001;

        public static void BuildWebGlPublication()
        {
            GoodPiratePublicationAudit.Run();
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.threadsSupport = false;
            PlayerSettings.WebGL.nameFilesAsHashes = true;
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            Environment.SetEnvironmentVariable("GP113_RELEASE_BUILD", "1");
            BuildWebGl();
        }

        public static void BuildWebGl()
        {
            var output = ReadArgument("-gp113BuildPath")
                ?? Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds/WebGL/dev"));
            var scenes = EnabledScenes();

            Directory.CreateDirectory(output);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);

            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);

            var options = BuildOptions.CleanBuildCache;
            if (Environment.GetEnvironmentVariable("GP113_RELEASE_BUILD") != "1")
            {
                options |= BuildOptions.Development;
            }
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = BuildTarget.WebGL,
                targetGroup = BuildTargetGroup.WebGL,
                options = options
            });

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"WebGL build failed with {report.summary.totalErrors} errors.");
            }

            Debug.Log(
                $"Good Pirate 1.13 WebGL build completed at {output}, "
                + $"{report.summary.totalSize} bytes.");
        }

        public static void BuildAndroidCheckpoint()
        {
            var output = ReadArgument("-gp113BuildPath")
                ?? Path.GetFullPath(Path.Combine(
                    Application.dataPath,
                    "../Builds/Android/.intermediates/"
                    + "good-pirate-1.13-r1-untrusted.apk"));
            var scenes = EnabledScenes();

            ConfigureAndroid();
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            EditorUserBuildSettings.buildAppBundle = false;

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.CleanBuildCache
            });

            var summary = report.summary;
            Debug.Log(
                $"GP113_ANDROID_CHECKPOINT result={summary.result} "
                + $"errors={summary.totalErrors} warnings={summary.totalWarnings} "
                + $"bytes={summary.totalSize} output={output}");
            if (summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Good Pirate Android build failed: {summary.result}");
            }
        }

        private static void ConfigureAndroid()
        {
            var android = NamedBuildTarget.Android;
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android, BuildTarget.Android);

            PlayerSettings.productName = ProductName;
            PlayerSettings.SetApplicationIdentifier(android, AndroidPackage);
            PlayerSettings.bundleVersion = VersionName;
            PlayerSettings.Android.bundleVersionCode = VersionCode;
            PlayerSettings.Android.minSdkVersion =
                AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetSdkVersion =
                AndroidSdkVersions.AndroidApiLevel36;
            PlayerSettings.SetScriptingBackend(
                android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures =
                AndroidArchitecture.ARM64;
            PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            PlayerSettings.SetManagedStrippingLevel(
                android, ManagedStrippingLevel.Low);

            PlayerSettings.SetUseDefaultGraphicsAPIs(
                BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(
                BuildTarget.Android,
                new[] { GraphicsDeviceType.OpenGLES3 });
            PlayerSettings.colorSpace = ColorSpace.Gamma;
            PlayerSettings.defaultInterfaceOrientation =
                UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.Android.applicationEntry =
                AndroidApplicationEntry.Activity;
            PlayerSettings.Android.forceInternetPermission = false;
            PlayerSettings.Android.forceSDCardPermission = false;
            PlayerSettings.Android.startInFullscreen = true;
            PlayerSettings.Android.renderOutsideSafeArea = true;
            PlayerSettings.Android.predictiveBackSupport = true;
            PlayerSettings.Android.useCustomKeystore = false;
            PlayerSettings.Android.keystoreName = string.Empty;
            PlayerSettings.Android.keyaliasName = string.Empty;
        }

        private static string[] EnabledScenes()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length != 3)
            {
                throw new InvalidOperationException(
                    $"Expected exactly three enabled 1.13 scenes, found {scenes.Length}.");
            }

            var expected = new[]
            {
                "Assets/00. Scenes/Loading.unity",
                "Assets/00. Scenes/MainGame.unity",
                "Assets/00. Scenes/Intro.unity"
            };
            if (!scenes.SequenceEqual(expected))
            {
                throw new InvalidOperationException(
                    "The enabled scene order does not match the recovered 1.13 build order.");
            }
            return scenes;
        }

        private static string ReadArgument(string name)
        {
            var arguments = Environment.GetCommandLineArgs();
            for (var index = 0; index + 1 < arguments.Length; index++)
            {
                if (arguments[index] == name)
                {
                    return Path.GetFullPath(arguments[index + 1]);
                }
            }

            return null;
        }
    }

    public sealed class GoodPirate113AndroidManifestPolicy :
        IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => int.MaxValue;

        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            var manifestPath = Path.Combine(
                basePath, "src", "main", "AndroidManifest.xml");
            var lines = File.ReadAllText(manifestPath)
                .Replace("\r\n", "\n")
                .Split('\n');
            var filtered = string.Join(
                "\n",
                lines.Where(line => !line.TrimStart().StartsWith(
                    "<uses-permission",
                    StringComparison.Ordinal))) + "\n";
            if (filtered.Contains("android.permission."))
            {
                throw new InvalidOperationException(
                    "Generated Android manifest still requests a permission");
            }
            File.WriteAllText(manifestPath, filtered);
            Debug.Log("GP113_ANDROID_MANIFEST removed Android permissions");
        }
    }
}
