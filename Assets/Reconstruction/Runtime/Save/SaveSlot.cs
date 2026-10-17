using System;

namespace GoodPirate113.Reconstruction.Save
{
    internal static class SaveSlot
    {
        public static void Validate(string slot)
        {
            if (string.IsNullOrWhiteSpace(slot))
            {
                throw new ArgumentException("A non-empty save slot is required.", nameof(slot));
            }

            for (var index = 0; index < slot.Length; index++)
            {
                var character = slot[index];
                var valid = character >= 'a' && character <= 'z'
                    || character >= 'A' && character <= 'Z'
                    || character >= '0' && character <= '9'
                    || character == '-'
                    || character == '_'
                    || character == '.';

                if (!valid)
                {
                    throw new ArgumentException(
                        "Save slots may contain only letters, digits, dot, dash, and underscore.",
                        nameof(slot));
                }
            }
        }
    }
}
