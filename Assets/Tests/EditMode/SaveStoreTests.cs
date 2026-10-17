using System;
using System.IO;
using GoodPirate113.Reconstruction.Core;
using GoodPirate113.Reconstruction.Save;
using NUnit.Framework;

namespace GoodPirate113.Reconstruction.Tests
{
    public sealed class SaveStoreTests
    {
        private string testDirectory;

        [SetUp]
        public void SetUp()
        {
            testDirectory = Path.Combine(
                Path.GetTempPath(),
                "good-pirate-113-save-tests-" + Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (!string.IsNullOrEmpty(testDirectory) && Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [Test]
        public void FileStoreRoundTripsOverwriteAndDelete()
        {
            var store = new FileSaveStore(testDirectory);
			var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
			var firstSave = GoodPirate113SaveV1.Create(clock);
			firstSave.PrepareForWrite(clock);
			var firstDocument = firstSave.Serialize();
			clock.UtcNow = clock.UtcNow.AddMinutes(1);
			var secondSave = GoodPirate113SaveV1.Create(clock);
			secondSave.PrepareForWrite(clock);
			var secondDocument = secondSave.Serialize();

            Assert.That(store.TryLoad("slot", out _), Is.False);
            store.Save("slot", firstDocument);
            Assert.That(store.TryLoad("slot", out var first), Is.True);
            Assert.That(first, Is.EqualTo(firstDocument));

            store.Save("slot", secondDocument);
            Assert.That(store.TryLoad("slot", out var second), Is.True);
            Assert.That(second, Is.EqualTo(secondDocument));
            Assert.That(store.TryLoadBackup("slot", out var backup), Is.True);
            Assert.That(backup, Is.EqualTo(firstDocument));

            store.Delete("slot");
            Assert.That(store.TryLoad("slot", out _), Is.False);
        }

        [Test]
        public void FileStoreRejectsDirectoryTraversalSlots()
        {
            var store = new FileSaveStore(testDirectory);

            Assert.Throws<ArgumentException>(() => store.Save("../outside", "payload"));
            Assert.That(Directory.Exists(testDirectory), Is.False);
        }

        [Test]
        public void WebGlStoreDelegatesToExplicitFallbackOutsidePlayer()
        {
            var fallback = new MemorySaveStore();
            var store = new WebGlLocalStorageSaveStore(fallback);

            store.Save("slot", "payload");
            Assert.That(store.TryLoad("slot", out var payload), Is.True);
            Assert.That(payload, Is.EqualTo("payload"));

            store.Delete("slot");
            Assert.That(store.TryLoad("slot", out _), Is.False);
        }

        [Test]
        public void RepositoryDoesNotOverwriteMalformedPayloadDuringRecovery()
        {
            var store = new MemorySaveStore { Payload = "not-json", HasPayload = true };
            var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
            var repository = new GoodPirate113SaveRepository(store, clock);

            var save = repository.LoadOrCreate(out var loadedExisting, out var warning);

            Assert.That(loadedExisting, Is.False);
            Assert.That(warning, Is.Not.Empty);
            Assert.That(save.Revision, Is.Zero);
            Assert.That(store.Payload, Is.EqualTo("not-json"));
        }

        [Test]
        public void RepositoryIncrementsRevisionOnlyWhenPersisting()
        {
            var store = new MemorySaveStore();
            var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
            var repository = new GoodPirate113SaveRepository(store, clock);
            var save = repository.LoadOrCreate(out _, out _);

            repository.Save(save);
            clock.UtcNow = clock.UtcNow.AddMinutes(1);
            repository.Save(save);

            Assert.That(save.Revision, Is.EqualTo(2));
            Assert.That(store.TryLoad(GoodPirate113SaveV1.DefaultSlot, out var payload), Is.True);
            Assert.That(GoodPirate113SaveV1.TryDeserialize(payload, out var restored, out var error), Is.True, error);
            Assert.That(restored.Revision, Is.EqualTo(2));
            Assert.That(restored.UpdatedUtc, Is.EqualTo("2026-08-27T12:31:00.0000000+00:00"));
        }

        [Test]
        public void RepositoryRestoresValidBackupWhenPrimaryIsCorrupt()
        {
            var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
            var valid = GoodPirate113SaveV1.Create(clock);
            valid.PrepareForWrite(clock);
            var store = new MemorySaveStore
            {
                HasPayload = true,
                Payload = "corrupt",
                HasBackup = true,
                Backup = valid.Serialize()
            };
            var repository = new GoodPirate113SaveRepository(store, clock);

            var restored = repository.LoadOrCreate(out var loadedExisting, out var warning);

            Assert.That(loadedExisting, Is.True);
            Assert.That(restored.Revision, Is.EqualTo(1));
            Assert.That(warning, Does.Contain("backup was restored"));
            Assert.That(GoodPirate113SaveV1.TryDeserialize(store.Payload, out _, out _), Is.True);
			Assert.That(store.HasBackup, Is.True);
			Assert.That(store.Backup, Is.EqualTo(valid.Serialize()));
        }

		[Test]
		public void FileStoreDoesNotReplaceValidBackupWithCorruptPrimary()
		{
			var store = new FileSaveStore(testDirectory);
			var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
			var first = GoodPirate113SaveV1.Create(clock);
			first.PrepareForWrite(clock);
			var firstDocument = first.Serialize();
			store.Save("slot", firstDocument);
			clock.UtcNow = clock.UtcNow.AddMinutes(1);
			var second = GoodPirate113SaveV1.Create(clock);
			second.PrepareForWrite(clock);
			store.Save("slot", second.Serialize());
			File.WriteAllText(Path.Combine(testDirectory, "slot.json"), "corrupt");
			clock.UtcNow = clock.UtcNow.AddMinutes(1);
			var third = GoodPirate113SaveV1.Create(clock);
			third.PrepareForWrite(clock);

			store.Save("slot", third.Serialize());

			Assert.That(store.TryLoadBackup("slot", out var backup), Is.True);
			Assert.That(backup, Is.EqualTo(firstDocument));
		}

		[Test]
		public void RepositoryRestoresProtectedPreImportSaveAsLastResort()
		{
			var store = new FileSaveStore(testDirectory);
			var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
			var protectedSave = GoodPirate113SaveV1.Create(clock);
			protectedSave.PrepareForWrite(clock);
			store.Save(GoodPirate113SaveV1.ImportRecoverySlot, protectedSave.Serialize());
			Directory.CreateDirectory(testDirectory);
			File.WriteAllText(Path.Combine(testDirectory, GoodPirate113SaveV1.DefaultSlot + ".json"), "corrupt");
			File.WriteAllText(Path.Combine(testDirectory, GoodPirate113SaveV1.DefaultSlot + ".json.bak"), "also-corrupt");

			var repository = new GoodPirate113SaveRepository(store, clock);
			var restored = repository.LoadOrCreate(out var loadedExisting, out var warning);

			Assert.That(loadedExisting, Is.True);
			Assert.That(restored.Revision, Is.EqualTo(1));
			Assert.That(warning, Does.Contain("pre-import save was restored"));
			Assert.That(store.TryLoad(GoodPirate113SaveV1.DefaultSlot, out var primary), Is.True);
			Assert.That(GoodPirate113SaveV1.TryDeserialize(primary, out _, out var error), Is.True, error);
		}

		[Test]
		public void OrdinarySavesDoNotOverwriteProtectedPreImportCopy()
		{
			var store = new FileSaveStore(testDirectory);
			var clock = new FakeClock(new DateTimeOffset(2026, 8, 27, 12, 30, 0, TimeSpan.Zero));
			var beforeImport = GoodPirate113SaveV1.Create(clock);
			beforeImport.PrepareForWrite(clock);
			var protectedDocument = beforeImport.Serialize();
			store.Save(GoodPirate113SaveV1.ImportRecoverySlot, protectedDocument);

			clock.UtcNow = clock.UtcNow.AddMinutes(1);
			var imported = GoodPirate113SaveV1.Create(clock);
			imported.PrepareForWrite(clock);
			store.Save(GoodPirate113SaveV1.DefaultSlot, imported.Serialize());
			clock.UtcNow = clock.UtcNow.AddMinutes(1);
			imported.PrepareForWrite(clock);
			store.Save(GoodPirate113SaveV1.DefaultSlot, imported.Serialize());

			Assert.That(store.TryLoad(GoodPirate113SaveV1.ImportRecoverySlot, out var stillProtected), Is.True);
			Assert.That(stillProtected, Is.EqualTo(protectedDocument));
		}

        private sealed class FakeClock : IUtcClock
        {
            public FakeClock(DateTimeOffset utcNow)
            {
                UtcNow = utcNow;
            }

            public DateTimeOffset UtcNow { get; set; }
        }

        private sealed class MemorySaveStore : ISaveStore
        {
            public bool HasPayload { get; set; }
            public string Payload { get; set; }
            public bool HasBackup { get; set; }
            public string Backup { get; set; }

            public bool TryLoad(string slot, out string payload)
            {
                payload = Payload;
                return HasPayload;
            }

            public void Save(string slot, string payload)
            {
                if (HasPayload)
                {
                    Backup = Payload;
                    HasBackup = true;
                }
                Payload = payload;
                HasPayload = true;
            }

            public bool TryLoadBackup(string slot, out string payload)
            {
                payload = Backup;
                return HasBackup;
            }

			public bool TryRestoreBackup(string slot, out string payload)
			{
				payload = Backup;
				if (!HasBackup)
				{
					return false;
				}
				Payload = Backup;
				HasPayload = true;
				return true;
			}

            public void Delete(string slot)
            {
                Payload = null;
                HasPayload = false;
                Backup = null;
                HasBackup = false;
            }
        }
    }
}
