using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Save
{
    [Serializable]
    public sealed class GoodPirate113BoolPreference
    {
        [SerializeField] private string key = string.Empty;
        [SerializeField] private bool value;

        public string Key => key;
        public bool Value => value;

        public GoodPirate113BoolPreference(string key, bool value)
        {
            this.key = key ?? string.Empty;
            this.value = value;
        }
    }

    [Serializable]
    public sealed class GoodPirate113IntPreference
    {
        [SerializeField] private string key = string.Empty;
        [SerializeField] private int value;

        public string Key => key;
        public int Value => value;

        public GoodPirate113IntPreference(string key, int value)
        {
            this.key = key ?? string.Empty;
            this.value = value;
        }
    }

    [Serializable]
    public sealed class GoodPirate113StringPreference
    {
        [SerializeField] private string key = string.Empty;
        [SerializeField] private string value = string.Empty;
        [SerializeField] private bool exists;

        public string Key => key;
        public string Value => value;
        public bool Exists => exists;

        public GoodPirate113StringPreference(string key, string value, bool exists)
        {
            this.key = key ?? string.Empty;
            this.value = value ?? string.Empty;
            this.exists = exists;
        }
    }

    [Serializable]
    public sealed class GoodPirate113PreferenceSnapshot
    {
        private const int MaximumEntries = 4096;
        private const int MaximumKeyLength = 160;
        private const int MaximumStringLength = 1024;

        [SerializeField] private List<GoodPirate113BoolPreference> boolValues =
            new List<GoodPirate113BoolPreference>();
        [SerializeField] private List<GoodPirate113IntPreference> intValues =
            new List<GoodPirate113IntPreference>();
        [SerializeField] private List<GoodPirate113StringPreference> stringValues =
            new List<GoodPirate113StringPreference>();

        [SerializeField] private bool isComplete;

        public IReadOnlyList<GoodPirate113BoolPreference> BoolValues => boolValues;
        public IReadOnlyList<GoodPirate113IntPreference> IntValues => intValues;
        public IReadOnlyList<GoodPirate113StringPreference> StringValues => stringValues;
        public bool IsEmpty => boolValues.Count == 0 && intValues.Count == 0 && stringValues.Count == 0;
        public bool IsComplete => isComplete;

        public void MarkComplete()
        {
            isComplete = true;
        }

        public void AddBool(string key, bool value)
        {
            boolValues.Add(new GoodPirate113BoolPreference(key, value));
        }

        public void AddInt(string key, int value)
        {
            intValues.Add(new GoodPirate113IntPreference(key, value));
        }

        public void AddString(string key, string value, bool exists)
        {
            stringValues.Add(new GoodPirate113StringPreference(key, value, exists));
        }

        public bool TryValidate(out string error)
        {
            Normalize();
            var count = boolValues.Count + intValues.Count + stringValues.Count;
            if (count > MaximumEntries)
            {
                error = "The preference snapshot contains too many values.";
                return false;
            }

            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var entry in boolValues)
            {
                if (!TryAcceptKey(entry == null ? null : entry.Key, keys, out error))
                {
                    return false;
                }
            }
            foreach (var entry in intValues)
            {
                if (!TryAcceptKey(entry == null ? null : entry.Key, keys, out error))
                {
                    return false;
                }
            }
            foreach (var entry in stringValues)
            {
                if (!TryAcceptKey(entry == null ? null : entry.Key, keys, out error))
                {
                    return false;
                }
                if (entry.Value != null && entry.Value.Length > MaximumStringLength)
                {
                    error = "A preference string is too long.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        internal void Normalize()
        {
            boolValues = boolValues ?? new List<GoodPirate113BoolPreference>();
            intValues = intValues ?? new List<GoodPirate113IntPreference>();
            stringValues = stringValues ?? new List<GoodPirate113StringPreference>();
        }

        private static bool TryAcceptKey(
            string key,
            HashSet<string> keys,
            out string error)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length > MaximumKeyLength)
            {
                error = "A preference key is invalid.";
                return false;
            }
            for (var index = 0; index < key.Length; index++)
            {
                if (char.IsControl(key[index]))
                {
                    error = "A preference key contains a control character.";
                    return false;
                }
            }
            if (!keys.Add(key))
            {
                error = "The preference snapshot contains a duplicate key: " + key;
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
