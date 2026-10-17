using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Save
{
    [Serializable]
    public sealed class GoodPirate113ProgressData
    {
        [SerializeField] private string playerDataBase64 = string.Empty;
        [SerializeField] private string inventoryJson = string.Empty;
        [SerializeField] private GoodPirate113PreferenceSnapshot preferences =
            new GoodPirate113PreferenceSnapshot();

        public string PlayerDataBase64
        {
            get => playerDataBase64;
            set => playerDataBase64 = value ?? string.Empty;
        }

        public string InventoryJson
        {
            get => inventoryJson;
            set => inventoryJson = value ?? string.Empty;
        }

        public GoodPirate113PreferenceSnapshot Preferences
        {
            get => preferences;
            set => preferences = value ?? new GoodPirate113PreferenceSnapshot();
        }

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public static bool TryDeserialize(
            string json,
            out GoodPirate113ProgressData progress,
            out string error)
        {
            progress = null;
            if (string.IsNullOrWhiteSpace(json) || json == "{}")
            {
                progress = new GoodPirate113ProgressData();
                error = string.Empty;
                return true;
            }

            try
            {
                progress = JsonUtility.FromJson<GoodPirate113ProgressData>(json);
            }
            catch (ArgumentException exception)
            {
                error = $"The progress payload is malformed: {exception.Message}";
                return false;
            }

            if (progress == null)
            {
                error = "The progress payload did not produce a progress object.";
                return false;
            }

            progress.playerDataBase64 = progress.playerDataBase64 ?? string.Empty;
            progress.inventoryJson = progress.inventoryJson ?? string.Empty;
            progress.preferences = progress.preferences ?? new GoodPirate113PreferenceSnapshot();
            if (!GoodPirate113PreferencePolicy.TryValidate(progress.preferences, out error))
            {
                progress = null;
                return false;
            }
            if (!TryValidatePlayerData(progress.playerDataBase64, out error)
                || !TryValidateInventory(progress.inventoryJson, out error))
            {
                progress = null;
                return false;
            }
            error = string.Empty;
            return true;
        }

        public bool TryValidateForTransfer(out string error)
        {
            if (string.IsNullOrWhiteSpace(playerDataBase64))
            {
                error = "The save does not contain player progress.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(inventoryJson))
            {
                error = "The save does not contain fleet inventory.";
                return false;
            }
            return TryValidatePlayerData(playerDataBase64, out error)
                && TryValidateInventory(inventoryJson, out error)
                && GoodPirate113PreferencePolicy.TryValidate(preferences, out error);
        }

        public void PrepareForImport()
        {
            preferences = preferences ?? new GoodPirate113PreferenceSnapshot();
            preferences.MarkComplete();
        }

        private static bool TryValidatePlayerData(string value, out string error)
        {
            if (string.IsNullOrEmpty(value))
            {
                error = string.Empty;
                return true;
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(value);
            }
            catch (FormatException)
            {
                error = "The player progress is not valid base64 data.";
                return false;
            }
            if (bytes.Length == 0 || bytes.Length > 1024 * 1024)
            {
                error = "The player progress has an invalid size.";
                return false;
            }

            try
            {
                var formatter = new BinaryFormatter
                {
                    Binder = new LegacyPlayerDataBinder()
                };
                using (var stream = new MemoryStream(bytes, false))
                {
                    var result = formatter.Deserialize(stream);
                    if (result == null || result.GetType().FullName != "MyData")
                    {
                        error = "The player progress has an unexpected root type.";
                        return false;
                    }
                    if (!TryValidateMyData(result, out error))
                    {
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                error = "The player progress is malformed: " + exception.Message;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryValidateInventory(string value, out string error)
        {
            if (string.IsNullOrEmpty(value))
            {
                error = string.Empty;
                return true;
            }
            if (value.Length > 2 * 1024 * 1024
                || value[0] != '{'
                || value[value.Length - 1] != '}')
            {
                error = "The fleet inventory has an invalid format.";
                return false;
            }

            try
            {
                var inventoryType = Type.GetType("MyInventoryData, Assembly-CSharp", false);
                if (inventoryType == null)
                {
                    error = "The fleet inventory type is unavailable.";
                    return false;
                }
                var inventory = JsonUtility.FromJson(value, inventoryType);
                if (inventory == null)
                {
                    error = "The fleet inventory did not produce an inventory object.";
                    return false;
                }
                if (!TryGetList(inventory, inventoryType, "myShipDataList", out var ships)
                    || ships.Count > 1000)
                {
                    error = "The fleet inventory is missing a valid myShipDataList.";
                    return false;
                }
                var shipIds = new HashSet<int>();
                foreach (var ship in ships)
                {
                    if (ship == null
                        || !TryReadInt(ship, "id", out var id)
                        || id < 0
                        || id >= 1000000
                        || !shipIds.Add(id)
                        || !TryReadInt(ship, "shipType", out var shipType)
                        || shipType <= 0
                        || shipType >= 14
                        || !TryValidateIntArray(ship, "equipSailerId", -1, 38)
                        || !TryValidateIntArray(ship, "lineIndex", -1, 23)
                        || !TryValidateIntArray(ship, "lineTempIndex", -1, 23))
                    {
                        error = "The fleet inventory contains an invalid ship entry.";
                        return false;
                    }
                }

                if (!TryGetList(inventory, inventoryType, "mySailorDataList", out var sailors)
                    || sailors.Count > 39)
                {
                    error = "The fleet inventory is missing a valid mySailorDataList.";
                    return false;
                }
                var sailorIds = new HashSet<int>();
                foreach (var sailor in sailors)
                {
                    if (sailor == null
                        || !TryReadInt(sailor, "sailorType", out var sailorType)
                        || sailorType < 0
                        || sailorType >= 39
                        || !sailorIds.Add(sailorType)
                        || !TryReadInt(sailor, "status", out var status)
                        || status < 0
                        || status > 2
                        || !TryReadInt(sailor, "level", out var level)
                        || level < 0
                        || level > 1000
                        || !TryReadInt(sailor, "haveCount", out var haveCount)
                        || haveCount < 0
                        || !TryReadInt(sailor, "exploreGrade", out var exploreGrade)
                        || exploreGrade < 0
                        || exploreGrade > 3)
                    {
                        error = "The fleet inventory contains an invalid sailor entry.";
                        return false;
                    }
                }

                if (!TryGetList(inventory, inventoryType, "myFormationList", out var formations)
                    || formations.Count != 3)
                {
                    error = "The fleet inventory must contain exactly three formations.";
                    return false;
                }
                foreach (var formation in formations)
                {
                    if (formation == null
                        || !TryReadInt(formation, "attackPoint", out var attack)
                        || !TryReadInt(formation, "healthPoint", out var health)
                        || !TryReadInt(formation, "speedPoint", out var speed)
                        || attack < 0
                        || health < 0
                        || speed < 0
                        || attack > 1000000
                        || health > 1000000
                        || speed > 1000000)
                    {
                        error = "The fleet inventory contains an invalid formation entry.";
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                error = "The fleet inventory is malformed: " + exception.Message;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryValidateMyData(object data, out string error)
        {
            if (!TryReadInt(data, "mainShipLevel", out var mainShipLevel)
                || mainShipLevel < 1
                || mainShipLevel > 1000
                || !TryReadInt(data, "selectedCubeNum", out var cube)
                || cube != 1
                || !TryReadInt(data, "selectedWorldNum", out var world)
                || world < 1
                || world > 6
                || !TryReadInt(data, "selectedIslandId", out var island)
                || island < 0
                || island >= 210
                || !TryReadInt(data, "selectedFormationNum", out var formation)
                || formation < 0
                || formation > 2
                || !TryReadInt(data, "selectedFlagId", out var flag)
                || flag < 0
                || flag > 39
                || !TryReadInt(data, "destinationIslandId", out var destination)
                || destination < -1
                || destination >= 210
                || !TryReadInt(data, "selectedSkin", out var skin)
                || skin < -1
                || skin > 20
                || !TryReadInt(data, "todaySelectedLegendarySailor", out var sailor)
                || sailor < 0
                || sailor >= 39)
            {
                error = "The player progress contains an out-of-range selection or level.";
                return false;
            }

            var type = data.GetType();
            var rewardSlots = GetFieldValue(type, data, "rewardBoxStatus") as Array;
            if (rewardSlots == null || rewardSlots.Length != 3)
            {
                error = "The player progress must contain exactly three reward box slots.";
                return false;
            }
            foreach (var reward in rewardSlots)
            {
                if (reward != null && !TryValidateRewardBox(reward))
                {
                    error = "The player progress contains an invalid reward box slot.";
                    return false;
                }
            }

            if (!TryGetList(data, type, "storeGetRewardBoxList", out var storeRewards)
                || storeRewards.Count > 10000)
            {
                error = "The player progress contains an invalid store reward list.";
                return false;
            }
            foreach (var reward in storeRewards)
            {
                if (reward == null || !TryValidateRewardBox(reward))
                {
                    error = "The player progress contains an invalid store reward entry.";
                    return false;
                }
            }

            if (!TryGetList(data, type, "storeCardList", out var cards)
                || cards.Count > 10000)
            {
                error = "The player progress contains an invalid store card list.";
                return false;
            }
            foreach (var card in cards)
            {
                if (card == null || Convert.ToInt32(card) < 0 || Convert.ToInt32(card) > 3)
                {
                    error = "The player progress contains an invalid store card entry.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        private static bool TryValidateRewardBox(object reward)
        {
            return TryReadInt(reward, "boxType", out var boxType)
                && boxType >= 0
                && boxType <= 4
                && TryReadInt(reward, "level", out var level)
                && level >= 0
                && level <= 1000;
        }

        private static bool TryValidateIntArray(object owner, string fieldName, int minimum, int maximum)
        {
            var array = GetFieldValue(owner.GetType(), owner, fieldName) as int[];
            if (array == null || array.Length != 3)
            {
                return false;
            }
            foreach (var value in array)
            {
                if (value < minimum || value > maximum)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TryGetList(object owner, Type type, string fieldName, out IList list)
        {
            list = GetFieldValue(type, owner, fieldName) as IList;
            return list != null;
        }

        private static bool TryReadInt(object owner, string fieldName, out int value)
        {
            value = 0;
            var raw = GetFieldValue(owner.GetType(), owner, fieldName);
            if (raw == null)
            {
                return false;
            }
            try
            {
                if (raw is int integer)
                {
                    value = integer;
                    return true;
                }
                if (raw.GetType().IsEnum)
                {
                    value = Convert.ToInt32(raw);
                    return true;
                }
                var decrypt = raw.GetType().GetMethod(
                    "GetDecrypted",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    Type.EmptyTypes,
                    null);
                if (decrypt != null)
                {
                    value = Convert.ToInt32(decrypt.Invoke(raw, null));
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private static object GetFieldValue(Type type, object owner, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            return field == null ? null : field.GetValue(owner);
        }

        private sealed class LegacyPlayerDataBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (!IsAllowedSerializedTypeName(assemblyName, typeName))
                {
                    throw new SerializationException(
                        "The player progress contains a disallowed type: " + typeName);
                }
                var resolved = Type.GetType(typeName + ", " + assemblyName, false);
                if (resolved == null || !IsAllowedType(resolved))
                {
                    throw new SerializationException(
                        "The player progress contains a disallowed type: " + typeName);
                }
                return resolved;
            }

            private static bool IsAllowedSerializedTypeName(string assemblyName, string typeName)
            {
                if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(typeName))
                {
                    return false;
                }
                if (assemblyName.StartsWith("Assembly-CSharp,", StringComparison.Ordinal))
                {
                    return typeName == "MyData"
                        || typeName == "RewardBoxStatus"
                        || typeName == "EGrade"
                        || typeName == "ERewardBox"
                        || typeName == "ESailor"
                        || typeName == "eShipSkin";
                }
                if (assemblyName.StartsWith("Assembly-CSharp-firstpass,", StringComparison.Ordinal))
                {
                    return typeName == "CodeStage.AntiCheat.ObscuredTypes.ObscuredBool"
                        || typeName == "CodeStage.AntiCheat.ObscuredTypes.ObscuredInt"
                        || typeName == "CodeStage.AntiCheat.ObscuredTypes.ObscuredLong";
                }
                if ((assemblyName.StartsWith("UnityEngine.CoreModule,", StringComparison.Ordinal)
                        || assemblyName.StartsWith("UnityEngine,", StringComparison.Ordinal))
                    && typeName == "UnityEngine.SystemLanguage")
                {
                    return true;
                }
                if (!assemblyName.StartsWith("mscorlib,", StringComparison.Ordinal)
                    && !assemblyName.StartsWith("System.Private.CoreLib,", StringComparison.Ordinal))
                {
                    return false;
                }
                return typeName == "System.String"
                    || typeName == "System.Decimal"
                    || typeName == "System.Int32"
                    || typeName == "System.Int64"
                    || typeName == "System.Boolean"
                    || typeName == "RewardBoxStatus[]"
                    || typeName.StartsWith("System.Collections.Generic.List`1[[RewardBoxStatus, Assembly-CSharp,", StringComparison.Ordinal)
                    || typeName.StartsWith("System.Collections.Generic.List`1[[EGrade, Assembly-CSharp,", StringComparison.Ordinal);
            }

            private static bool IsAllowedType(Type type)
            {
                if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
                {
                    return true;
                }
                if (type.IsArray)
                {
                    return IsAllowedType(type.GetElementType());
                }
                if (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
                {
                    return IsAllowedType(type.GetGenericArguments()[0]);
                }
                if (type.IsEnum)
                {
                    return type.Assembly.GetName().Name == "Assembly-CSharp"
                        || type.FullName == "UnityEngine.SystemLanguage";
                }
                if (type.Assembly.GetName().Name == "Assembly-CSharp")
                {
                    return type.FullName == "MyData" || type.FullName == "RewardBoxStatus";
                }
                if (type.Assembly.GetName().Name == "Assembly-CSharp-firstpass")
                {
                    return type.FullName == "CodeStage.AntiCheat.ObscuredTypes.ObscuredBool"
                        || type.FullName == "CodeStage.AntiCheat.ObscuredTypes.ObscuredInt"
                        || type.FullName == "CodeStage.AntiCheat.ObscuredTypes.ObscuredLong";
                }
                return false;
            }
        }
    }
}
