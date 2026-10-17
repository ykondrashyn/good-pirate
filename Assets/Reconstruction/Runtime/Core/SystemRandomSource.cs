using System;

namespace GoodPirate113.Reconstruction.Core
{
    public sealed class SystemRandomSource : IRandomSource
    {
        private readonly object sync = new object();
        private readonly Random random;

        public SystemRandomSource()
            : this(Environment.TickCount)
        {
        }

        public SystemRandomSource(int seed)
        {
            random = new Random(seed);
        }

        public int NextInt(int minimumInclusive, int maximumExclusive)
        {
            if (maximumExclusive <= minimumInclusive)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumExclusive),
                    "The exclusive maximum must be greater than the inclusive minimum.");
            }

            lock (sync)
            {
                return random.Next(minimumInclusive, maximumExclusive);
            }
        }

        public float NextFloat01()
        {
            lock (sync)
            {
                const int precision = 1 << 24;
                return random.Next(0, precision) / (float)precision;
            }
        }
    }
}
