using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GoodPirate113.Reconstruction.Save
{
    public sealed class WebGlLocalStorageSaveStore : ISaveStore
    {
        private readonly ISaveStore nonWebGlFallback;

        public WebGlLocalStorageSaveStore(ISaveStore nonWebGlFallback = null)
        {
            this.nonWebGlFallback = nonWebGlFallback;
        }

        public bool TryLoad(string slot, out string payload)
        {
            SaveSlot.Validate(slot);

#if UNITY_WEBGL && !UNITY_EDITOR
            return TryLoadBrowser(slot, out payload);
#else
            return RequireFallback().TryLoad(slot, out payload);
#endif
        }

        public bool TryLoadBackup(string slot, out string payload)
        {
            SaveSlot.Validate(slot);

#if UNITY_WEBGL && !UNITY_EDITOR
            return TryLoadBrowser(BackupSlot(slot), out payload);
#else
            return RequireFallback().TryLoadBackup(slot, out payload);
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        private static bool TryLoadBrowser(string key, out string payload)
        {
            var byteLength = GoodPirate113StorageGetLength(key);
            if (byteLength == -1)
            {
                payload = string.Empty;
                return false;
            }

            if (byteLength < -1)
            {
                throw new InvalidOperationException("Browser localStorage is unavailable.");
            }

            var buffer = new byte[byteLength + 1];
            if (GoodPirate113StorageRead(key, buffer, buffer.Length) != 1)
            {
                throw new InvalidOperationException("Browser localStorage could not be read.");
            }

            payload = Encoding.UTF8.GetString(buffer, 0, byteLength);
            return true;
        }
#endif

        public void Save(string slot, string payload)
        {
            SaveSlot.Validate(slot);
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            if (TryLoadBrowser(slot, out var previous)
                && GoodPirate113SaveV1.TryDeserialize(previous, out _, out _)
                && GoodPirate113StorageWrite(BackupSlot(slot), previous) != 1)
            {
                throw new InvalidOperationException("Browser save backup could not be written.");
            }

            if (GoodPirate113StorageWrite(slot, payload) != 1)
            {
                throw new InvalidOperationException("Browser localStorage could not be written.");
            }
#else
            RequireFallback().Save(slot, payload);
#endif
        }

        public bool TryRestoreBackup(string slot, out string payload)
        {
            SaveSlot.Validate(slot);
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!TryLoadBrowser(BackupSlot(slot), out payload)
                || !GoodPirate113SaveV1.TryDeserialize(payload, out _, out _))
            {
                payload = string.Empty;
                return false;
            }
            if (GoodPirate113StorageWrite(slot, payload) != 1)
            {
                throw new InvalidOperationException("Browser save backup could not be restored.");
            }
            return true;
#else
            return RequireFallback().TryRestoreBackup(slot, out payload);
#endif
        }

        public void Delete(string slot)
        {
            SaveSlot.Validate(slot);

#if UNITY_WEBGL && !UNITY_EDITOR
            if (GoodPirate113StorageDelete(slot) != 1)
            {
                throw new InvalidOperationException("Browser localStorage could not be cleared.");
            }

            if (GoodPirate113StorageDelete(BackupSlot(slot)) != 1)
            {
                throw new InvalidOperationException("Browser save backup could not be cleared.");
            }
#else
            RequireFallback().Delete(slot);
#endif
        }

        private ISaveStore RequireFallback()
        {
            return nonWebGlFallback
                ?? throw new PlatformNotSupportedException(
                    "The localStorage adapter requires a fallback outside a WebGL player.");
        }

        private static string BackupSlot(string slot)
        {
            return slot + ".backup";
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GoodPirate113StorageGetLength(string key);

        [DllImport("__Internal")]
        private static extern int GoodPirate113StorageRead(string key, byte[] buffer, int bufferLength);

        [DllImport("__Internal")]
        private static extern int GoodPirate113StorageWrite(string key, string payload);

        [DllImport("__Internal")]
        private static extern int GoodPirate113StorageDelete(string key);
#endif
    }
}
