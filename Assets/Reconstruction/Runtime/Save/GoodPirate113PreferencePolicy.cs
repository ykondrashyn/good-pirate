using System;
using System.Collections.Generic;
using System.Globalization;

namespace GoodPirate113.Reconstruction.Save
{
    public static class GoodPirate113PreferencePolicy
    {
        private static readonly HashSet<string> ShipNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "Normal", "Cannon", "Rapid", "Repair", "Fishing", "SuicideBomb",
            "Guide", "Harpoon", "Multi", "Gas", "Elite", "Bomber", "Ghost"
        };

        private static readonly HashSet<string> SailorNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "boatswain_a_0", "boatswain_a_1", "boatswain_b_0", "boatswain_b_1",
            "boatswain_b_2", "boatswain_b_3", "boatswain_c_0", "boatswain_c_1",
            "boatswain_c_2", "boatswain_c_3", "boatswain_c_4", "boatswain_c_5",
            "boatswain_s_0", "boatswain_s_1", "gunner_a_0", "gunner_a_1",
            "gunner_b_0", "gunner_b_1", "gunner_c_0", "gunner_c_1", "gunner_c_2",
            "gunner_s_0", "sailing_a_0", "sailing_b_0", "sailing_c_0", "sailing_c_1",
            "sailing_s_0", "steersman_s_0", "steersman_a_0", "steersman_b_0",
            "boatswain_s_inappp", "skill_meteor", "skill_spark", "skill_windblast",
            "skill_avoidEnvironment", "skill_unlimitSubship", "skill_SubshipGenerateFast",
            "skill_healthDown", "skill_timeIsGold"
        };

        private static readonly HashSet<string> TutorialNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "start", "move_base", "goldbox", "firstbattle", "battlewasperfect",
            "worldmap", "doubletap", "rewardbox", "not used", "crewpopup",
            "upgradepopup", "heal", "battleguide", "secondbattle", "firstisland",
            "openclaimpopup", "winclaimfirst"
        };

        private static readonly int[] PatrolCounts = { 3, 2, 2, 2, 2, 3 };

        public static bool TryValidate(GoodPirate113PreferenceSnapshot snapshot, out string error)
        {
            if (snapshot == null)
            {
                error = "The preference snapshot is missing.";
                return false;
            }
            if (!snapshot.TryValidate(out error))
            {
                return false;
            }

            foreach (var entry in snapshot.BoolValues)
            {
                if (!IsAllowedBoolKey(entry.Key))
                {
                    error = "The save contains an unsupported boolean preference: " + entry.Key;
                    return false;
                }
            }
            foreach (var entry in snapshot.IntValues)
            {
                if (!IsAllowedIntKey(entry.Key))
                {
                    error = "The save contains an unsupported integer preference: " + entry.Key;
                    return false;
                }
                if (entry.Value < 0 || entry.Value > 1000000)
                {
                    error = "The save contains an invalid integer preference: " + entry.Key;
                    return false;
                }
            }
            foreach (var entry in snapshot.StringValues)
            {
                if (!IsAllowedStringKey(entry.Key))
                {
                    error = "The save contains an unsupported string preference: " + entry.Key;
                    return false;
                }
                if (entry.Exists && !IsValidCooldown(entry.Value))
                {
                    error = "The save contains an invalid cooldown preference: " + entry.Key;
                    return false;
                }
                if (!entry.Exists && !string.IsNullOrEmpty(entry.Value))
                {
                    error = "A missing cooldown preference contains a value: " + entry.Key;
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public static bool IsAllowedBoolKey(string key)
        {
            if (TryParseDoorKey(key))
            {
                return true;
            }
            if (HasNumberInRange(key, "Flag_", 0, 39))
            {
                return true;
            }
            if (TryExtract(key, "Ship_", "_IsHave", out var shipName)
                && ShipNames.Contains(shipName))
            {
                return true;
            }
            if (TryExtract(key, string.Empty, "_isNew", out shipName)
                && ShipNames.Contains(shipName))
            {
                return true;
            }
            if (TryExtract(key, "Tutorial_", string.Empty, out var tutorialName)
                && TutorialNames.Contains(tutorialName))
            {
                return true;
            }
            if (TryParseBossKey(key))
            {
                return true;
            }
            return HasNumberInRange(key, "Cube_Island_", 0, 209)
                || HasNumberInRange(key, "Island_Claim_", 0, 209)
                || HasNumberInRange(key, "ShipSkin_", 0, 20)
                || HasNumberInRange(key, "World_", 1, 6, "_Reward");
        }

        public static bool IsAllowedIntKey(string key)
        {
            if (HasNumberInRange(key, "Flag_", 0, 39, "_ADS"))
            {
                return true;
            }
            return TryExtract(key, "Ship_", "_Level", out var shipName)
                && ShipNames.Contains(shipName);
        }

        public static bool IsAllowedStringKey(string key)
        {
            if (!TryExtract(key, "COOLTIME_", string.Empty, out var suffix))
            {
                return false;
            }
            if (suffix == "KEY_FREEDIA"
                || suffix == "GetDouble"
                || suffix == "SkillMeteor"
                || suffix == "SkillSpark"
                || suffix == "SkillWindblast"
                || ShipNames.Contains(suffix))
            {
                return true;
            }
            if (HasNumberInRange(suffix, "RewardBoxSlot_", 0, 2)
                || HasNumberInRange(suffix, "ADSFLAGCOOLTIME", 0, 39)
                || TryParsePatrolSuffix(suffix))
            {
                return true;
            }
            if (TryExtract(suffix, "SailorExplore_", string.Empty, out var sailorName))
            {
                return SailorNames.Contains(sailorName);
            }
            return int.TryParse(suffix, NumberStyles.None, CultureInfo.InvariantCulture, out var shipId)
                && shipId >= 0
                && shipId < 1000000;
        }

        private static bool TryParseDoorKey(string key)
        {
            const string prefix = "Cube1_Island_";
            const string marker = "_Door_";
            if (string.IsNullOrEmpty(key) || !key.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }
            var markerIndex = key.IndexOf(marker, prefix.Length, StringComparison.Ordinal);
            return markerIndex > prefix.Length
                && int.TryParse(key.Substring(prefix.Length, markerIndex - prefix.Length), out var island)
                && int.TryParse(key.Substring(markerIndex + marker.Length), out var door)
                && island >= 1
                && island <= 6
                && door >= 0
                && door < 4;
        }

        private static bool TryParseBossKey(string key)
        {
            const string prefix = "Boss_1_";
            const string suffix = "_0";
            return TryExtract(key, prefix, suffix, out var worldText)
                && int.TryParse(worldText, out var world)
                && world >= 1
                && world <= 6;
        }

        private static bool TryParsePatrolSuffix(string suffix)
        {
            const string prefix = "Patrol_1_";
            if (string.IsNullOrEmpty(suffix) || !suffix.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }
            var parts = suffix.Substring(prefix.Length).Split('_');
            if (parts.Length != 2
                || !int.TryParse(parts[0], out var world)
                || !int.TryParse(parts[1], out var patrol)
                || world < 1
                || world > PatrolCounts.Length)
            {
                return false;
            }
            return patrol >= 0 && patrol < PatrolCounts[world - 1];
        }

        private static bool IsValidCooldown(string value)
        {
            if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var binary))
            {
                return false;
            }
            try
            {
                DateTime.FromBinary(binary);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static bool HasNumberInRange(
            string key,
            string prefix,
            int minimum,
            int maximum,
            string suffix = "")
        {
            return TryExtract(key, prefix, suffix, out var number)
                && int.TryParse(number, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
                && value >= minimum
                && value <= maximum;
        }

        private static bool TryExtract(string value, string prefix, string suffix, out string middle)
        {
            middle = string.Empty;
            if (value == null
                || !value.StartsWith(prefix, StringComparison.Ordinal)
                || !value.EndsWith(suffix, StringComparison.Ordinal)
                || value.Length < prefix.Length + suffix.Length)
            {
                return false;
            }
            middle = value.Substring(prefix.Length, value.Length - prefix.Length - suffix.Length);
            return middle.Length > 0;
        }
    }
}
