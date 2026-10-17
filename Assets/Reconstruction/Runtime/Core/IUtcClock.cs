using System;

namespace GoodPirate113.Reconstruction.Core
{
    public interface IUtcClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
