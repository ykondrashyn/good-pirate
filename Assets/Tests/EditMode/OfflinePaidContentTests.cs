using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoodPirate113.Reconstruction.Offline;
using NUnit.Framework;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Tests
{
    public sealed class OfflinePaidContentTests
    {
        [Test]
        public void CatalogContainsEveryRecoveredPaidProductExactlyOnce()
        {
            var products = GoodPirateOfflineProductCatalog.All;

            Assert.That(products.Count, Is.EqualTo(16));
            Assert.That(products.Select(product => product.Id).Distinct().Count(), Is.EqualTo(16));
            Assert.That(products.All(product => !string.IsNullOrWhiteSpace(product.Id)), Is.True);
            Assert.That(products.Count(product => product.IsRestorable), Is.EqualTo(6));
            Assert.That(products.Count(product => product.IsRepeatable), Is.EqualTo(6));
            Assert.That(products.Count(product => product.Kind == OfflineProductKind.OneTimePack), Is.EqualTo(3));
            Assert.That(products.Count(product => product.Kind == OfflineProductKind.DailyPack), Is.EqualTo(1));
        }

        [Test]
        public void CrystalProductsRetainRecoveredRepeatableRewards()
        {
            var expected = new Dictionary<string, int>
            {
                [GoodPirateOfflineProductCatalog.Crystal00] = 100,
                [GoodPirateOfflineProductCatalog.Crystal01] = 300,
                [GoodPirateOfflineProductCatalog.Crystal02] = 1000,
                [GoodPirateOfflineProductCatalog.Crystal03] = 2100,
                [GoodPirateOfflineProductCatalog.Crystal04] = 3900,
                [GoodPirateOfflineProductCatalog.Crystal05] = 7400
            };

            foreach (var pair in expected)
            {
                Assert.That(GoodPirateOfflineProductCatalog.TryGet(pair.Key, out var product), Is.True);
                Assert.That(product.IsRepeatable, Is.True, pair.Key);
                Assert.That(product.DiamondAmount, Is.EqualTo(pair.Value), pair.Key);
            }
        }

        [Test]
        public void LoadingSceneSerializesAllCatalogProductIds()
        {
            var scene = ReadAsset("00. Scenes/Loading.unity");

            foreach (var product in GoodPirateOfflineProductCatalog.All)
            {
                Assert.That(Count(scene, product.Id), Is.EqualTo(1), product.Id);
            }
        }

        [Test]
        public void MainGameWiresEveryPaidButtonAndRestoreExactlyOnce()
        {
            var scene = ReadAsset("00. Scenes/MainGame.unity");
            var callbacks = new[]
            {
                "BuyPackStarter", "BuyPackToday", "BuyPackBirds",
                "BuyPackIntermediate", "BuyPackExpert", "BuyParrot",
                "BuyEagle", "BuyBlackBird", "BuyRemoveAds", "BuyCrystal00",
                "BuyCrystal01", "BuyCrystal02", "BuyCrystal03", "BuyCrystal04",
                "BuyCrystal05", "BuyShipSkinRaft", "Restore"
            };

            foreach (var callback in callbacks)
            {
                Assert.That(Count(scene, "m_MethodName: " + callback), Is.EqualTo(1), callback);
            }
        }

        [Test]
        public void StoreUsesLocalGrantsAndExposesRestore()
        {
            var iap = ReadAsset("Scripts/Assembly-CSharp/IapManager.cs");
            var store = ReadAsset("Scripts/Assembly-CSharp/UI_Store.cs");

            Assert.That(iap, Does.Contain("TryBuyOfflineProduct(productId)"));
            Assert.That(iap, Does.Contain("CanGrantOfflineProduct(productId)"));
			Assert.That(iap, Does.Contain("TryBuyOfflineProduct(args.purchasedProduct.definition.id)"));
            Assert.That(iap, Does.Not.Contain("Unavailable(\"billing\", \"buy-"));
            Assert.That(store, Does.Contain("restoreBtn.SetActive(value: true)"));
            Assert.That(store, Does.Contain("isHavePirrot &&"));
        }

        [Test]
        public void SaveTransferUsesValidatedEnvelopeAndRaftReconciliation()
        {
            var settings = ReadAsset("Scripts/Assembly-CSharp/UI_SettingPopup.cs");
            var bootstrap = ReadAsset("Reconstruction/Runtime/Bootstrap/GoodPirateRuntimeBootstrap.cs");
            var playerInfo = ReadAsset("Scripts/Assembly-CSharp/PlayerInfo.cs");
            var template = ReadAsset("WebGLTemplates/GoodPirate113/index.html");

            Assert.That(settings, Does.Contain("ExportSaveDocument"));
            Assert.That(settings, Does.Contain("TryImportSaveDocument"));
            Assert.That(settings, Does.Not.Contain("GoogleCloudManager.Instance.GoogleCloud"));
            Assert.That(bootstrap, Does.Contain("GoodPirate113SaveV1.TryDeserialize"));
            Assert.That(playerInfo, Does.Contain("if (myData.isBuyRaft)"));
			Assert.That(settings, Does.Contain("RequestImportConfirmation"));
			Assert.That(settings, Does.Contain("RearmManualPaste"));
			Assert.That(settings, Does.Contain("GoodPirate113PortablePreferences.Capture"));
            Assert.That(template, Does.Contain("id=\"clipboard-panel\""));
			Assert.That(template, Does.Contain("Replace local save"));
			Assert.That(template, Does.Contain("goodpirate113-import-result"));
			Assert.That(settings, Does.Contain("EnsureSaveButtonLabel(\"btn_cloud up\", \"Copy\\nSave\")"));
			Assert.That(settings, Does.Contain("EnsureSaveButtonLabel(\"btn_cloud down\", \"Paste\\nSave\")"));
			Assert.That(ReadAsset("00. Scenes/MainGame.unity"), Does.Contain("The local save was not changed"));
        }

        private static string ReadAsset(string relativePath)
        {
            var fullPath = Path.Combine(Application.dataPath, relativePath);
            Assert.That(File.Exists(fullPath), Is.True, fullPath);
            return File.ReadAllText(fullPath);
        }

        private static int Count(string text, string value)
        {
            var count = 0;
            var start = 0;
            while ((start = text.IndexOf(value, start, StringComparison.Ordinal)) >= 0)
            {
                count++;
                start += value.Length;
            }
            return count;
        }
    }
}
