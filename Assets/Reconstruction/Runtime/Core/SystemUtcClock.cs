using System;

namespace GoodPirate113.Reconstruction.Core
{
    public sealed class SystemUtcClock : IUtcClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
