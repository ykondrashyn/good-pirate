using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Tests
{
    public sealed class PublicationCompatibilityTests
    {
        private static Type ValueType(string name) => Type.GetType("CodeStage.AntiCheat.ObscuredTypes." + name + ", Assembly-CSharp-firstpass", true);

        [TestCase("ObscuredInt", "{\"currentCryptoKey\":444444,\"hiddenValue\":444470,\"inited\":true}", "42")]
        [TestCase("ObscuredLong", "{\"currentCryptoKey\":444442,\"hiddenValue\":444464,\"inited\":true}", "42")]
        [TestCase("ObscuredBool", "{\"currentCryptoKey\":215,\"hiddenValue\":2,\"inited\":true}", "True")]
        [TestCase("ObscuredBool", "{\"currentCryptoKey\":215,\"hiddenValue\":98,\"inited\":true}", "False")]
        public void RecoveredSerializedFieldLayoutRetainsItsValue(string name, string json, string expected)
        {
            var value = JsonUtility.FromJson(json, ValueType(name));
            Assert.That(value.ToString(), Is.EqualTo(expected));
        }

        [TestCase("ObscuredInt", "0")]
        [TestCase("ObscuredLong", "0")]
        [TestCase("ObscuredBool", "False")]
        public void UninitializedCompatibilityValueHasRecoveredDefault(string name, string expected)
        {
            Assert.That(Activator.CreateInstance(ValueType(name)).ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void LegacySdkAssembliesCannotBeLoaded()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.GetName().Name;
                Assert.That(name, Is.Not.EqualTo("DOTweenPro"));
                Assert.That(name, Is.Not.EqualTo("Facebook.Unity"));
                Assert.That(name, Is.Not.EqualTo("UnityEngine.Purchasing"));
                Assert.That(name, Is.Not.EqualTo("Security"));
            }
        }
    }
}
