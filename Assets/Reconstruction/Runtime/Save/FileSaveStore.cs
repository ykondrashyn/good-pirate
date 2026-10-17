using System;
using System.IO;
using System.Text;

namespace GoodPirate113.Reconstruction.Save
{
    public sealed class FileSaveStore : ISaveStore
    {
        private readonly string rootDirectory;

        public FileSaveStore(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                throw new ArgumentException("A save directory is required.", nameof(rootDirectory));
            }

            this.rootDirectory = Path.GetFullPath(rootDirectory);
        }

        public bool TryLoad(string slot, out string payload)
        {
            SaveSlot.Validate(slot);
            return TryLoadPath(GetPath(slot), out payload);
        }

        public bool TryLoadBackup(string slot, out string payload)
        {
            SaveSlot.Validate(slot);
            return TryLoadPath(GetPath(slot) + ".bak", out payload);
        }

        private static bool TryLoadPath(string path, out string payload)
        {
            if (!File.Exists(path))
            {
                payload = string.Empty;
                return false;
            }

            payload = File.ReadAllText(path, Encoding.UTF8);
            return true;
        }

        public void Save(string slot, string payload)
        {
            SaveSlot.Validate(slot);
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            Directory.CreateDirectory(rootDirectory);
            var path = GetPath(slot);
            var temporaryPath = path + ".tmp-" + Guid.NewGuid().ToString("N");
            var backupPath = path + ".bak";

            try
            {
                using (var stream = new FileStream(
                    temporaryPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    FileOptions.WriteThrough))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(payload);
                    writer.Flush();
                    stream.Flush(true);
                }

                if (File.Exists(path)
                    && TryLoadPath(path, out var previous)
                    && GoodPirate113SaveV1.TryDeserialize(previous, out _, out _))
                {
                    ReplaceExistingFile(temporaryPath, path, backupPath);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        File.Copy(temporaryPath, path, true);
                    }
                    else
                    {
                        File.Move(temporaryPath, path);
                    }
                }
            }
            finally
            {
                DeleteIfPresent(temporaryPath);
            }
        }

        public bool TryRestoreBackup(string slot, out string payload)
        {
            SaveSlot.Validate(slot);
            Directory.CreateDirectory(rootDirectory);
            var path = GetPath(slot);
            var backupPath = path + ".bak";
            if (!TryLoadPath(backupPath, out payload)
                || !GoodPirate113SaveV1.TryDeserialize(payload, out _, out _))
            {
                payload = string.Empty;
                return false;
            }

            var temporaryPath = path + ".restore-" + Guid.NewGuid().ToString("N");
            try
            {
                File.Copy(backupPath, temporaryPath, true);
                if (File.Exists(path))
                {
                    File.Copy(temporaryPath, path, true);
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
                return true;
            }
            finally
            {
                DeleteIfPresent(temporaryPath);
            }
        }

        public void Delete(string slot)
        {
            SaveSlot.Validate(slot);
            var path = GetPath(slot);
            DeleteIfPresent(path);
            DeleteIfPresent(path + ".bak");
        }

        private string GetPath(string slot)
        {
            return Path.Combine(rootDirectory, slot + ".json");
        }

        private static void ReplaceExistingFile(string temporaryPath, string path, string backupPath)
        {
            DeleteIfPresent(backupPath);

            try
            {
                File.Replace(temporaryPath, path, backupPath, true);
            }
            catch (PlatformNotSupportedException)
            {
                File.Copy(path, backupPath, true);
                File.Copy(temporaryPath, path, true);
            }
            catch (IOException)
            {
                File.Copy(path, backupPath, true);
                File.Copy(temporaryPath, path, true);
            }
        }

        private static void DeleteIfPresent(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
