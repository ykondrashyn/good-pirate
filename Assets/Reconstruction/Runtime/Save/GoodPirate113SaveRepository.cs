using System;
using GoodPirate113.Reconstruction.Core;

namespace GoodPirate113.Reconstruction.Save
{
    public sealed class GoodPirate113SaveRepository
    {
        private readonly ISaveStore store;
        private readonly IUtcClock clock;
        private readonly string slot;

        public GoodPirate113SaveRepository(ISaveStore store, IUtcClock clock, string slot = GoodPirate113SaveV1.DefaultSlot)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
            SaveSlot.Validate(slot);
            this.slot = slot;
        }

        public GoodPirate113SaveV1 LoadOrCreate(out bool loadedExisting, out string warning)
        {
            loadedExisting = false;
            warning = string.Empty;

            var primaryWarning = string.Empty;
            try
            {
                if (store.TryLoad(slot, out var payload))
                {
                    if (GoodPirate113SaveV1.TryDeserialize(payload, out var primary, out primaryWarning))
                    {
                        loadedExisting = true;
                        return primary;
                    }
                }
            }
            catch (Exception exception)
            {
                primaryWarning = $"The primary save could not be read: {exception.Message}";
            }

            try
            {
                var backupWarning = string.Empty;
                if (store.TryLoadBackup(slot, out var backupPayload)
                    && GoodPirate113SaveV1.TryDeserialize(backupPayload, out var backup, out backupWarning))
                {
                    if (!store.TryRestoreBackup(slot, out var restoredPayload)
                        || !GoodPirate113SaveV1.TryDeserialize(restoredPayload, out backup, out backupWarning))
                    {
                        throw new InvalidOperationException("The valid save backup could not be restored.");
                    }
                    loadedExisting = true;
                    warning = string.IsNullOrEmpty(primaryWarning)
                        ? "The primary save was unavailable. A valid backup was restored."
                        : primaryWarning + " A valid backup was restored.";
                    return backup;
                }

                if (!string.IsNullOrEmpty(backupWarning))
                {
                    warning = string.IsNullOrEmpty(primaryWarning)
                        ? backupWarning
                        : primaryWarning + " " + backupWarning;
                }
            }
            catch (Exception exception)
            {
                warning = string.IsNullOrEmpty(primaryWarning)
                    ? $"The save backup could not be read: {exception.Message}"
                    : primaryWarning + $" The save backup could not be read: {exception.Message}";
            }

            try
            {
                if (store.TryLoad(GoodPirate113SaveV1.ImportRecoverySlot, out var recoveryPayload)
                    && GoodPirate113SaveV1.TryDeserialize(recoveryPayload, out var recovery, out _))
                {
                    store.Save(slot, recoveryPayload);
                    loadedExisting = true;
                    warning = string.IsNullOrEmpty(warning)
                        ? "The primary and backup saves were unavailable. The pre-import save was restored."
                        : warning + " The pre-import save was restored.";
                    return recovery;
                }
            }
            catch (Exception exception)
            {
                warning = string.IsNullOrEmpty(warning)
                    ? $"The pre-import save could not be read: {exception.Message}"
                    : warning + $" The pre-import save could not be read: {exception.Message}";
            }

            if (string.IsNullOrEmpty(warning))
            {
                warning = primaryWarning;
            }

            return GoodPirate113SaveV1.Create(clock);
        }

        public void Save(GoodPirate113SaveV1 save)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            save.PrepareForWrite(clock);
            store.Save(slot, save.Serialize());
        }

        public void Delete()
        {
            store.Delete(slot);
        }
    }
}
