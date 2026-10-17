using GoodPirate113.Reconstruction.Bootstrap;

namespace GoodPirate113.Reconstruction.Save
{
    public static class GoodPirate113LegacyStateBridge
    {
        public static bool TryLoadPlayerData(out string playerDataBase64)
        {
            playerDataBase64 = string.Empty;
            if (!TryGetProgress(out var progress))
            {
                return false;
            }

            playerDataBase64 = progress.PlayerDataBase64;
            return !string.IsNullOrEmpty(playerDataBase64);
        }

        public static bool TryLoadInventoryData(out string inventoryJson)
        {
            inventoryJson = string.Empty;
            if (!TryGetProgress(out var progress))
            {
                return false;
            }

            inventoryJson = progress.InventoryJson;
            return !string.IsNullOrEmpty(inventoryJson);
        }

        public static bool TrySavePlayerData(string playerDataBase64)
        {
            if (!TryGetRuntimeAndProgress(out var runtime, out var progress))
            {
                return false;
            }

            progress.PlayerDataBase64 = playerDataBase64;
            runtime.CurrentSave.PayloadJson = progress.Serialize();
            runtime.SaveNow();
            return true;
        }

        public static bool TrySaveInventoryData(string inventoryJson)
        {
            if (!TryGetRuntimeAndProgress(out var runtime, out var progress))
            {
                return false;
            }

            progress.InventoryJson = inventoryJson;
            runtime.CurrentSave.PayloadJson = progress.Serialize();
            runtime.SaveNow();
            return true;
        }

        public static bool TryLoadPreferences(out GoodPirate113PreferenceSnapshot preferences)
        {
            preferences = null;
            if (!TryGetProgress(out var progress)
                || progress.Preferences == null
                || progress.Preferences.IsEmpty && !progress.Preferences.IsComplete)
            {
                return false;
            }

            preferences = progress.Preferences;
            return true;
        }

        public static bool TrySavePreferences(GoodPirate113PreferenceSnapshot preferences)
        {
            if (preferences == null
                || !GoodPirate113PreferencePolicy.TryValidate(preferences, out _)
                || !TryGetRuntimeAndProgress(out var runtime, out var progress))
            {
                return false;
            }

            progress.Preferences = preferences;
            runtime.CurrentSave.PayloadJson = progress.Serialize();
            runtime.SaveNow();
            return true;
        }

        private static bool TryGetProgress(out GoodPirate113ProgressData progress)
        {
            var runtime = GoodPirateRuntimeBootstrap.Instance;
            if (runtime == null || runtime.CurrentSave == null)
            {
                progress = null;
                return false;
            }

            return GoodPirate113ProgressData.TryDeserialize(
                runtime.CurrentSave.PayloadJson,
                out progress,
                out _);
        }

        private static bool TryGetRuntimeAndProgress(
            out GoodPirateRuntimeBootstrap runtime,
            out GoodPirate113ProgressData progress)
        {
            runtime = GoodPirateRuntimeBootstrap.Instance;
            if (runtime == null || runtime.CurrentSave == null)
            {
                progress = null;
                return false;
            }

            return GoodPirate113ProgressData.TryDeserialize(
                runtime.CurrentSave.PayloadJson,
                out progress,
                out _);
        }
    }
}
