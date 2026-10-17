using System;
using GoodPirate113.Reconstruction.Core;
using GoodPirate113.Reconstruction.Save;
using NUnit.Framework;

namespace GoodPirate113.Reconstruction.Tests
{
    public sealed class SaveModelTests
    {
        [Test]
        public void CreateAndRoundTripPreservesEnvelopeAndProgressPayload()
        {
            var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
            var save = GoodPirate113SaveV1.Create(clock);
			var progress = new GoodPirate113ProgressData();
			progress.Preferences.AddBool("ShipSkin_20", true);
			var payload = progress.Serialize();
			save.PayloadJson = payload;
            save.PrepareForWrite(clock);

            var json = save.Serialize();

            Assert.That(GoodPirate113SaveV1.TryDeserialize(json, out var restored, out var error), Is.True, error);
            Assert.That(restored.SchemaVersion, Is.EqualTo(GoodPirate113SaveV1.CurrentSchemaVersion));
            Assert.That(restored.Revision, Is.EqualTo(1));
			Assert.That(restored.PayloadJson, Is.EqualTo(payload));
            Assert.That(restored.CreatedUtc, Is.EqualTo("2026-08-27T12:30:00.0000000+00:00"));
        }

        [Test]
        public void DeserializeRejectsUnsupportedSchemaWithoutReturningObject()
        {
            var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
            var json = GoodPirate113SaveV1.Create(clock)
                .Serialize()
                .Replace("\"schemaVersion\":1", "\"schemaVersion\":2");

            var valid = GoodPirate113SaveV1.TryDeserialize(json, out var save, out var error);

            Assert.That(valid, Is.False);
            Assert.That(save, Is.Null);
            Assert.That(error, Does.Contain("Unsupported save schema 2"));
        }

        [Test]
        public void DeserializeRejectsPayloadChangedWithoutChecksumUpdate()
        {
            var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
            var save = GoodPirate113SaveV1.Create(clock);
            save.PayloadJson = "{\"gold\":17}";
            save.PrepareForWrite(clock);
            var corrupted = save.Serialize().Replace("{\\\"gold\\\":17}", "{\\\"gold\\\":18}");

            Assert.That(
                GoodPirate113SaveV1.TryDeserialize(corrupted, out var restored, out var error),
                Is.False);
            Assert.That(restored, Is.Null);
            Assert.That(error, Does.Contain("checksum"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("{}")]
        public void DeserializeRejectsIncompleteDocuments(string json)
        {
            Assert.That(
                GoodPirate113SaveV1.TryDeserialize(json, out var save, out var error),
                Is.False);
            Assert.That(save, Is.Null);
            Assert.That(error, Is.Not.Empty);
        }

        [Test]
        public void ProgressPayloadRoundTripKeepsPortablePreferences()
        {
			var progress = new GoodPirate113ProgressData();
			progress.Preferences.AddBool("ShipSkin_20", true);
			progress.Preferences.AddInt("Ship_Normal_Level", 4);
			progress.Preferences.AddString("COOLTIME_KEY_FREEDIA", "12345", true);

            var json = progress.Serialize();

            Assert.That(
                GoodPirate113ProgressData.TryDeserialize(json, out var restored, out var error),
                Is.True,
                error);
			Assert.That(restored.Preferences.BoolValues[0].Key, Is.EqualTo("ShipSkin_20"));
			Assert.That(restored.Preferences.BoolValues[0].Value, Is.True);
			Assert.That(restored.Preferences.IntValues[0].Value, Is.EqualTo(4));
			Assert.That(restored.Preferences.StringValues[0].Exists, Is.True);
        }

		[Test]
		public void ProgressPayloadRejectsMalformedNestedPlayerData()
		{
			var progress = new GoodPirate113ProgressData
			{
				PlayerDataBase64 = "not-base64"
			};

			Assert.That(
				GoodPirate113ProgressData.TryDeserialize(progress.Serialize(), out _, out var error),
				Is.False);
			Assert.That(error, Does.Contain("base64"));
		}

		[Test]
		public void PreferenceSnapshotRejectsDuplicateKeysAcrossTypes()
		{
			var snapshot = new GoodPirate113PreferenceSnapshot();
			snapshot.AddBool("same-key", true);
			snapshot.AddInt("same-key", 1);

			Assert.That(snapshot.TryValidate(out var error), Is.False);
			Assert.That(error, Does.Contain("duplicate"));
		}

		[Test]
		public void PreferencePolicyRejectsUnknownKeysAndMalformedCooldowns()
		{
			var unknown = new GoodPirate113PreferenceSnapshot();
			unknown.AddBool("Boss_1_99_0", true);
			Assert.That(GoodPirate113PreferencePolicy.TryValidate(unknown, out var unknownError), Is.False);
			Assert.That(unknownError, Does.Contain("unsupported"));

			var cooldown = new GoodPirate113PreferenceSnapshot();
			cooldown.AddString("COOLTIME_RewardBoxSlot_0", "not-a-date", true);
			Assert.That(GoodPirate113PreferencePolicy.TryValidate(cooldown, out var cooldownError), Is.False);
			Assert.That(cooldownError, Does.Contain("cooldown"));
		}

		[Test]
		public void CompletePreferenceSnapshotRoundTripsEvenWhenItHasNoValues()
		{
			var progress = new GoodPirate113ProgressData();
			progress.Preferences.MarkComplete();

			Assert.That(
				GoodPirate113ProgressData.TryDeserialize(progress.Serialize(), out var restored, out var error),
				Is.True,
				error);
			Assert.That(restored.Preferences.IsComplete, Is.True);
			Assert.That(restored.Preferences.IsEmpty, Is.True);
		}

		[Test]
		public void ProgressPayloadRejectsNullInventoryEntriesAndWrongFormationCount()
		{
			var nullShip = new GoodPirate113ProgressData
			{
				InventoryJson = "{\"myShipDataList\":[null],\"mySailorDataList\":[],\"myFormationList\":[{},{},{}]}"
			};
			Assert.That(
				GoodPirate113ProgressData.TryDeserialize(nullShip.Serialize(), out _, out var shipError),
				Is.False);
			Assert.That(shipError, Does.Contain("ship entry"));

			var twoFormations = new GoodPirate113ProgressData
			{
				InventoryJson = "{\"myShipDataList\":[],\"mySailorDataList\":[],\"myFormationList\":[{},{}]}"
			};
			Assert.That(
				GoodPirate113ProgressData.TryDeserialize(twoFormations.Serialize(), out _, out var formationError),
				Is.False);
			Assert.That(formationError, Does.Contain("exactly three formations"));
		}

        [Test]
        public void EmptyEnvelopePayloadCreatesFreshProgressComposition()
        {
            Assert.That(
                GoodPirate113ProgressData.TryDeserialize("{}", out var progress, out var error),
                Is.True,
                error);
            Assert.That(progress.PlayerDataBase64, Is.Empty);
            Assert.That(progress.InventoryJson, Is.Empty);
        }

        private sealed class FakeClock : IUtcClock
        {
            public FakeClock(DateTimeOffset utcNow)
            {
                UtcNow = utcNow;
            }

            public DateTimeOffset UtcNow { get; }
        }
    }
}
