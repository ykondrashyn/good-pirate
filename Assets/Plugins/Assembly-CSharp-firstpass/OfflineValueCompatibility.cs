// Serialization-compatible value adapters. No anti-cheat package is required.
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
    [Serializable]
    public struct ObscuredInt : IFormattable, IEquatable<ObscuredInt>
    {
        [SerializeField] private int currentCryptoKey;
        [SerializeField] private int hiddenValue;
        [SerializeField] private bool inited;
        [SerializeField] private int fakeValue;
        [SerializeField] private bool fakeValueActive;
        private int Value => inited ? hiddenValue ^ currentCryptoKey : 0;
        public static implicit operator int(ObscuredInt v) => v.Value;
        public static implicit operator ObscuredInt(int v) => new ObscuredInt { currentCryptoKey = 444444, hiddenValue = v ^ 444444, inited = true, fakeValue = v, fakeValueActive = false };
        public static ObscuredInt operator ++(ObscuredInt v) => v.Value + 1;
        public static ObscuredInt operator --(ObscuredInt v) => v.Value - 1;
        public int GetDecrypted() => Value;
        public bool Equals(ObscuredInt other) => Value == other.Value;
        public override bool Equals(object other) => other is ObscuredInt value && Equals(value);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
        public string ToString(string format) => Value.ToString(format);
        public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);
    }

    [Serializable]
    public struct ObscuredLong : IFormattable, IEquatable<ObscuredLong>
    {
        [SerializeField] private long currentCryptoKey;
        [SerializeField] private long hiddenValue;
        [SerializeField] private bool inited;
        [SerializeField] private long fakeValue;
        [SerializeField] private bool fakeValueActive;
        private long Value => inited ? hiddenValue ^ currentCryptoKey : 0L;
        public static implicit operator long(ObscuredLong v) => v.Value;
        public static implicit operator ObscuredLong(long v) => new ObscuredLong { currentCryptoKey = 444442L, hiddenValue = v ^ 444442L, inited = true, fakeValue = v, fakeValueActive = false };
        public static ObscuredLong operator ++(ObscuredLong v) => v.Value + 1L;
        public static ObscuredLong operator --(ObscuredLong v) => v.Value - 1L;
        public long GetDecrypted() => Value;
        public bool Equals(ObscuredLong other) => Value == other.Value;
        public override bool Equals(object other) => other is ObscuredLong value && Equals(value);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
        public string ToString(string format) => Value.ToString(format);
        public string ToString(string format, IFormatProvider provider) => Value.ToString(format, provider);
    }

    [Serializable]
    public struct ObscuredBool : IEquatable<ObscuredBool>
    {
        [SerializeField] private byte currentCryptoKey;
        [SerializeField] private int hiddenValue;
        [SerializeField] private bool inited;
        [SerializeField] private bool fakeValue;
        [SerializeField] private bool fakeValueActive;
        private bool Value => inited && (hiddenValue ^ currentCryptoKey) != 181;
        public static implicit operator bool(ObscuredBool v) => v.Value;
        public static implicit operator ObscuredBool(bool v) => new ObscuredBool { currentCryptoKey = 215, hiddenValue = (v ? 213 : 181) ^ 215, inited = true, fakeValue = v, fakeValueActive = false };
        public bool GetDecrypted() => Value;
        public bool Equals(ObscuredBool other) => Value == other.Value;
        public override bool Equals(object other) => other is ObscuredBool value && Equals(value);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
    }

    public static class ObscuredPrefs
    {
        private static string Key(string key) => "gp113-local:" + key;
        public static bool HasKey(string key) => PlayerPrefs.HasKey(Key(key));
        public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(Key(key));
        public static void DeleteAll() => PlayerPrefs.DeleteAll();
        public static void Save() => PlayerPrefs.Save();
        public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(Key(key), defaultValue);
        public static void SetInt(string key, int value) => PlayerPrefs.SetInt(Key(key), value);
        public static bool GetBool(string key, bool defaultValue = false) => GetInt(key, defaultValue ? 1 : 0) != 0;
        public static void SetBool(string key, bool value) => SetInt(key, value ? 1 : 0);
        public static string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(Key(key), defaultValue);
        public static void SetString(string key, string value) => PlayerPrefs.SetString(Key(key), value);
    }
}
