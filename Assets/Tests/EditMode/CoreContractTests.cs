using GoodPirate113.Reconstruction.Core;
using GoodPirate113.Reconstruction.Input;
using GoodPirate113.Reconstruction.Platform;
using GoodPirate113.Reconstruction.Rendering;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Tests
{
    public sealed class CoreContractTests
    {
        [Test]
        public void SeededRandomSourcesProduceSameBoundedSequence()
        {
            var first = new SystemRandomSource(113);
            var second = new SystemRandomSource(113);

            for (var index = 0; index < 32; index++)
            {
                var firstValue = first.NextInt(-4, 9);
                var secondValue = second.NextInt(-4, 9);
                Assert.That(firstValue, Is.EqualTo(secondValue));
                Assert.That(firstValue, Is.InRange(-4, 8));
            }
        }

        [Test]
        public void RandomFloatRemainsInsideHalfOpenUnitInterval()
        {
            var random = new SystemRandomSource(113);

            for (var index = 0; index < 4096; index++)
            {
                Assert.That(random.NextFloat01(), Is.GreaterThanOrEqualTo(0f).And.LessThan(1f));
            }
        }

        [Test]
        public void PointerSampleClampsPressureWithoutChangingCoordinates()
        {
            var sample = new GoodPiratePointerSample(
                7,
                GoodPiratePointerPhase.Moved,
                new Vector2(10f, 20f),
                new Vector2(2f, -3f),
                4f,
                1.5,
                true);

            Assert.That(sample.Pressure01, Is.EqualTo(1f));
            Assert.That(sample.ScreenPosition, Is.EqualTo(new Vector2(10f, 20f)));
            Assert.That(sample.ScreenDelta, Is.EqualTo(new Vector2(2f, -3f)));
            Assert.That(sample.IsPrimary, Is.True);
        }

        [Test]
        public void PinchSamplePreservesSignedDistanceDelta()
        {
            var sample = new GoodPiratePinchSample(Vector2.one, 100f, -12f, 3.0);

            Assert.That(sample.Distance, Is.EqualTo(100f));
            Assert.That(sample.DistanceDelta, Is.EqualTo(-12f));
        }

        [Test]
        public void BrowserPresentationTimingUsesRequestAnimationFrame()
        {
            Assert.That(
                GoodPirateFrameRatePolicy.ResolveTargetFrameRate(30, true),
                Is.EqualTo(GoodPirateFrameRatePolicy.BrowserManagedFrameRate));
            Assert.That(
                GoodPirateFrameRatePolicy.ResolveTargetFrameRate(60, true),
                Is.EqualTo(GoodPirateFrameRatePolicy.BrowserManagedFrameRate));
        }

        [TestCase(30)]
        [TestCase(60)]
        public void NativePresentationTimingRetainsRecoveredPreference(int target)
        {
            Assert.That(
                GoodPirateFrameRatePolicy.ResolveTargetFrameRate(target, false),
                Is.EqualTo(target));
        }

        [Test]
        public void WaterMaterialUsesPinnedDokdoAppearanceContract()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/Material/WaterProDaytime.mat");
            var foam = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Texture2D/foam.png");

            Assert.That(material, Is.Not.Null);
            Assert.That(foam, Is.Not.Null);
            Assert.That(material.shader.name, Is.EqualTo("FX/Water"));
            Assert.That(material.GetTexture("_FoamTex"), Is.SameAs(foam));
            Assert.That(material.GetFloat("_FoamScale"), Is.EqualTo(0.018f).Within(0.0001f));
            Assert.That(material.GetFloat("_FoamStrength"), Is.EqualTo(0.22f).Within(0.0001f));
            Assert.That(material.GetFloat("_WaterAlpha"), Is.EqualTo(0.94f).Within(0.0001f));
            Assert.That(material.GetFloat("_WaveScale"), Is.EqualTo(0.02f).Within(0.0001f));
        }

        [Test]
        public void RecoveredSteeringMeshRebuildsRoundedRootDome()
        {
            var source = AssetDatabase.LoadAssetAtPath<Mesh>(
                "Assets/Mesh/Sphere001.asset");
            var rebuilt = GoodPirateSteeringMesh
                .CreateCorrectedTeardrop(source);

            Assert.That(source, Is.Not.Null);
            Assert.That(source.vertexCount, Is.EqualTo(296));
            Assert.That(source.bindposes.Length, Is.EqualTo(2));
            Assert.That(rebuilt, Is.Not.Null);
            Assert.That(rebuilt.vertexCount, Is.EqualTo(448));
            Assert.That(rebuilt.triangles.Length,
                Is.EqualTo(source.triangles.Length));
            Assert.That(rebuilt.boneWeights.Skip(source.vertexCount)
                .All(weight => weight.boneIndex0 == 0 &&
                    Mathf.Approximately(weight.weight0, 1f)), Is.True);

            Object.DestroyImmediate(rebuilt);
        }

        [Test]
        public void SteeringSkinningUsesBothRecoveredBones()
        {
            var rendererObject = new GameObject("steering renderer");
            var renderer = rendererObject.AddComponent<
                SkinnedMeshRenderer>();
            var previousSkinWeights = QualitySettings.skinWeights;
            try
            {
                QualitySettings.skinWeights = SkinWeights.OneBone;
                renderer.quality = SkinQuality.Bone1;

                GoodPirateSteeringSkinning.EnsureTwoBoneBlending(renderer);

                Assert.That(QualitySettings.skinWeights,
                    Is.EqualTo(SkinWeights.TwoBones));
                Assert.That(renderer.quality,
                    Is.EqualTo(SkinQuality.Bone2));
            }
            finally
            {
                QualitySettings.skinWeights = previousSkinWeights;
                Object.DestroyImmediate(rendererObject);
            }
        }

        [Test]
        public void JoystickTextureUsesStableWebGlSampling()
        {
            var importer = AssetImporter.GetAtPath(
                "Assets/Texture2D/UI_ring003.png")
                as TextureImporter;
            Assert.That(importer, Is.Not.Null);

            var webGl = importer.GetPlatformTextureSettings(
                "WebGL");

            Assert.That(importer.mipmapEnabled, Is.False);
            Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Clamp));
            Assert.That(importer.alphaIsTransparency, Is.True);
            Assert.That(webGl.overridden, Is.True);
            Assert.That(webGl.maxTextureSize, Is.EqualTo(256));
            Assert.That(webGl.format, Is.EqualTo(TextureImporterFormat.RGBA32));
            Assert.That(webGl.textureCompression,
                Is.EqualTo(TextureImporterCompression.Uncompressed));
        }
    }
}
