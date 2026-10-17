using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using GoodPirate113.Reconstruction.Core;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Save
{
    [Serializable]
    public sealed class GoodPirate113SaveV1
    {
        public const int CurrentSchemaVersion = 1;
        public const string DefaultSlot = "good-pirate-1.13-save-v1";
        public const string ImportRecoverySlot = "good-pirate-1.13-save-v1.pre-import";

        [SerializeField] private int schemaVersion = CurrentSchemaVersion;
        [SerializeField] private long revision;
        [SerializeField] private string createdUtc = string.Empty;
        [SerializeField] private string updatedUtc = string.Empty;
        [SerializeField] private string payloadJson = "{}";
        [SerializeField] private string checksumSha256 = string.Empty;

        public int SchemaVersion => schemaVersion;
        public long Revision => revision;
        public string CreatedUtc => createdUtc;
        public string UpdatedUtc => updatedUtc;
        public string ChecksumSha256 => checksumSha256;

        public string PayloadJson
        {
            get => payloadJson;
            set
            {
                payloadJson = string.IsNullOrWhiteSpace(value) ? "{}" : value;
                RefreshChecksum();
            }
        }

        public static GoodPirate113SaveV1 Create(IUtcClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            var timestamp = FormatTimestamp(clock.UtcNow);
            var save = new GoodPirate113SaveV1
            {
                schemaVersion = CurrentSchemaVersion,
                revision = 0,
                createdUtc = timestamp,
                updatedUtc = timestamp,
                payloadJson = "{}"
            };
            save.RefreshChecksum();
            return save;
        }

        public void PrepareForWrite(IUtcClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (revision == long.MaxValue)
            {
                throw new InvalidOperationException("The save revision cannot be incremented further.");
            }

            if (string.IsNullOrWhiteSpace(createdUtc))
            {
                createdUtc = FormatTimestamp(clock.UtcNow);
            }

            revision++;
            updatedUtc = FormatTimestamp(clock.UtcNow);
            RefreshChecksum();
        }

        public string Serialize()
        {
            if (!TryValidate(out var error))
            {
                throw new InvalidOperationException(error);
            }

            return JsonUtility.ToJson(this);
        }

        public bool TryValidate(out string error)
        {
            if (schemaVersion != CurrentSchemaVersion)
            {
                error = $"Unsupported save schema {schemaVersion}.";
                return false;
            }

            if (revision < 0)
            {
                error = "The save revision cannot be negative.";
                return false;
            }

            if (!IsRoundTripTimestamp(createdUtc) || !IsRoundTripTimestamp(updatedUtc))
            {
                error = "Save timestamps must be valid UTC timestamps.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(payloadJson))
            {
                error = "The save payload cannot be empty.";
                return false;
            }

            var expectedChecksum = ComputeChecksum();
            if (!FixedTimeEquals(checksumSha256, expectedChecksum))
            {
                error = "The save checksum does not match its contents.";
                return false;
            }

            if (!GoodPirate113ProgressData.TryDeserialize(payloadJson, out _, out error))
            {
                error = "The save payload is invalid. " + error;
                return false;
            }

            error = string.Empty;
            return true;
        }

        public static bool TryDeserialize(
            string json,
            out GoodPirate113SaveV1 save,
            out string error)
        {
            save = null;

            if (string.IsNullOrWhiteSpace(json))
            {
                error = "The save document is empty.";
                return false;
            }

            try
            {
                save = JsonUtility.FromJson<GoodPirate113SaveV1>(json);
            }
            catch (ArgumentException exception)
            {
                error = $"The save document is malformed: {exception.Message}";
                return false;
            }

            if (save == null)
            {
                error = "The save document did not produce a save object.";
                return false;
            }

            if (!save.TryValidate(out error))
            {
                save = null;
                return false;
            }

            return true;
        }

        private static string FormatTimestamp(DateTimeOffset timestamp)
        {
            return timestamp.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
        }

        private void RefreshChecksum()
        {
            checksumSha256 = ComputeChecksum();
        }

        private string ComputeChecksum()
        {
            var canonical = string.Join(
                "\n",
                schemaVersion.ToString(CultureInfo.InvariantCulture),
                revision.ToString(CultureInfo.InvariantCulture),
                createdUtc ?? string.Empty,
                updatedUtc ?? string.Empty,
                payloadJson ?? string.Empty);
            using (var algorithm = SHA256.Create())
            {
                var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(canonical));
                var result = new StringBuilder(bytes.Length * 2);
                foreach (var value in bytes)
                {
                    result.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        private static bool IsRoundTripTimestamp(string value)
        {
            return DateTimeOffset.TryParseExact(
                value,
                "O",
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var parsed)
                && parsed.Offset == TimeSpan.Zero;
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            if (left == null || right == null || left.Length != right.Length)
            {
                return false;
            }
            var difference = 0;
            for (var index = 0; index < left.Length; index++)
            {
                difference |= left[index] ^ right[index];
            }
            return difference == 0;
        }
    }
}
