namespace GoodPirate113.Reconstruction.Save
{
    public interface ISaveStore
    {
        bool TryLoad(string slot, out string payload);

        bool TryLoadBackup(string slot, out string payload);

        void Save(string slot, string payload);

        bool TryRestoreBackup(string slot, out string payload);

        void Delete(string slot);
    }
}
