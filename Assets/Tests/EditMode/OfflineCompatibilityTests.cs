using System;
using System.IO;
using System.Reflection;
using GoodPirate113.Reconstruction.Offline;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace GoodPirate113.Reconstruction.Tests
{
    public sealed class OfflineCompatibilityTests
    {
        [Test]
        public void PolicyPublishesDeterministicDisposition()
        {
            OfflineServiceOperation received = default;
            var calls = 0;
            Action<OfflineServiceOperation> listener = operation =>
            {
                received = operation;
                calls++;
            };

            OfflineServicePolicy.OperationHandled += listener;
            try
            {
                OfflineServicePolicy.Unavailable("cloud", "load");
            }
            finally
            {
                OfflineServicePolicy.OperationHandled -= listener;
            }

            Assert.That(calls, Is.EqualTo(1));
            Assert.That(received.Service, Is.EqualTo("cloud"));
            Assert.That(received.Operation, Is.EqualTo("load"));
            Assert.That(received.Disposition, Is.EqualTo(OfflineServiceDisposition.Unavailable));
        }

        [Test]
        public void RecoveredServiceContractsRemainPresent()
        {
            AssertMethod("Appsflyer, Assembly-CSharp", "ValidateReceipt", 4);
            AssertMethod("Appsflyer, Assembly-CSharp", "SendEvent", 1);
            AssertMethod("GoogleCloudManager, Assembly-CSharp", "GoogleCloudSave", 0);
            AssertMethod("GoogleCloudManager, Assembly-CSharp", "GoogleCloudLoad", 0);
            AssertMethod("ICloudManager, Assembly-CSharp", "ICloudSave", 0);
            AssertMethod("ICloudManager, Assembly-CSharp", "ICloudLoad", 0);
            AssertMethod("IapManager, Assembly-CSharp", "InitializePurchasing", 0);
            AssertMethod("IapManager, Assembly-CSharp", "BuyProductID", 1);
            AssertMethod("IapManager, Assembly-CSharp", "ProcessPurchase", 1);
            AssertMethod("MoPubAdManager, Assembly-CSharp", "ShowRewardedVideo", 1);
            AssertMethod("MoPubAdManager, Assembly-CSharp", "ShowInterstitialAd", 0);
            AssertMethod("MoPubBinding, Assembly-CSharp", "RequestRewardedVideo", 6);
            AssertMethod("MoPubBinding, Assembly-CSharp", "ShowRewardedVideo", 1);
            AssertMethod("MoPubiOS, Assembly-CSharp", "InitializeSdk", 1);
            AssertMethod("PercentCloud, Assembly-CSharp", "RequestBackup", 3);
            AssertMethod("SocialPlatformManager, Assembly-CSharp", "ReportLeaderBoardScore", 2);
            AssertMethod("GoogleAnalyticsV3, Assembly-CSharp-firstpass", "LogEvent", 4);

            var iap = RequireType("IapManager, Assembly-CSharp");
            Assert.That(iap.GetField("inapp_removeads"), Is.Not.Null);
            Assert.That(iap.GetField("inapp_crystal_05"), Is.Not.Null);
            Assert.That(iap.GetField("loadingBlockOn"), Is.Not.Null);
            Assert.That(iap.GetField("loadingBlockOff"), Is.Not.Null);

            var ads = RequireType("MoPubAdManager, Assembly-CSharp");
            Assert.That(ads.GetField("_bannerAdUnitsAndroid", BindingFlags.Instance | BindingFlags.NonPublic), Is.Not.Null);
            Assert.That(ads.GetField("_interstitialAdUnitsAndroid", BindingFlags.Instance | BindingFlags.NonPublic), Is.Not.Null);
            Assert.That(ads.GetField("_rewardedVideoAdUnitsAndroid", BindingFlags.Instance | BindingFlags.NonPublic), Is.Not.Null);
        }

        [Test]
        public void RewardedAdInvokesOneLocalRewardWithoutMoPubBridge()
        {
            var adManagerType = RequireType("MoPubAdManager, Assembly-CSharp");
            var gameObject = new GameObject("OfflineRewardedAdTest");
            var rewardCalls = 0;
            var policyCalls = 0;
            OfflineServiceOperation received = default;
            Action<OfflineServiceOperation> listener = operation =>
            {
                if (operation.Service == "ads" && operation.Operation == "show-rewarded-video")
                {
                    received = operation;
                    policyCalls++;
                }
            };

            OfflineServicePolicy.OperationHandled += listener;
            try
            {
                var component = gameObject.AddComponent(adManagerType);
                UnityAction callback = () => rewardCalls++;
                adManagerType.GetMethod("ShowRewardedVideo")?.Invoke(component, new object[] { callback });
            }
            finally
            {
                OfflineServicePolicy.OperationHandled -= listener;
                UnityEngine.Object.DestroyImmediate(gameObject);
            }

            Assert.That(rewardCalls, Is.EqualTo(1));
            Assert.That(policyCalls, Is.EqualTo(1));
            Assert.That(received.Disposition, Is.EqualTo(OfflineServiceDisposition.LocalReward));
        }

        [Test]
        public void SceneFacingSourcesContainNoLiveServiceDispatch()
        {
            AssertSourceOmits("Scripts/Assembly-CSharp/MoPubAdManager.cs", "MoPubAndroid.");
            AssertSourceOmits("Scripts/Assembly-CSharp/MoPubBinding.cs", "DllImport");
            AssertSourceOmits("Scripts/Assembly-CSharp/MoPubiOS.cs", "DllImport");
            AssertSourceOmits("Scripts/Assembly-CSharp/Appsflyer.cs", "AppsFlyer.");
            AssertSourceOmits("Scripts/Assembly-CSharp/IapManager.cs", "UnityPurchasing.Initialize");
            AssertSourceOmits("Scripts/Assembly-CSharp/IapManager.cs", "new global::UnityEngine.WWW");
            AssertSourceOmits("Scripts/Assembly-CSharp/Percent/Http/HttpsClient.cs", "new global::UnityEngine.WWW");
            Assert.That(File.Exists(Path.Combine(Application.dataPath, "Plugins/Assembly-CSharp-firstpass/GoogleAnalyticsAndroidV3.cs")), Is.False);
        }

        [Test]
        public void PreSceneInitializersDoNotCallRemovedNativePlugins()
        {
            Assert.That(Directory.Exists(Path.Combine(Application.dataPath, "Plugins/Assembly-CSharp-firstpass/SA/Android"))
                && Directory.GetFiles(Path.Combine(Application.dataPath, "Plugins/Assembly-CSharp-firstpass/SA/Android"), "*.cs", SearchOption.AllDirectories).Length > 0, Is.False);
            AssertSourceOmits("Plugins/Assembly-CSharp-firstpass/OfflineServiceCompatibility.cs", "DllImport");
            AssertSourceOmits("Plugins/Assembly-CSharp-firstpass/OfflineServiceCompatibility.cs", "AndroidJavaClass");
        }

        private static Type RequireType(string qualifiedName)
        {
            var type = Type.GetType(qualifiedName, throwOnError: false);
            Assert.That(type, Is.Not.Null, qualifiedName);
            return type;
        }

        private static void AssertMethod(string qualifiedTypeName, string methodName, int parameterCount)
        {
            var type = RequireType(qualifiedTypeName);
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.That(Array.Exists(methods, method => method.Name == methodName && method.GetParameters().Length == parameterCount), Is.True,
                qualifiedTypeName + "." + methodName);
        }

        private static void AssertSourceOmits(string relativePath, string forbiddenText)
        {
            var fullPath = Path.Combine(Application.dataPath, relativePath);
            Assert.That(File.Exists(fullPath), Is.True, fullPath);
            Assert.That(File.ReadAllText(fullPath), Does.Not.Contain(forbiddenText));
        }

        private static void AssertSourceContains(string relativePath, string requiredText)
        {
            var fullPath = Path.Combine(Application.dataPath, relativePath);
            Assert.That(File.Exists(fullPath), Is.True, fullPath);
            Assert.That(File.ReadAllText(fullPath), Does.Contain(requiredText));
        }
    }
}
