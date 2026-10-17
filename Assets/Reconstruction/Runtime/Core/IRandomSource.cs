namespace GoodPirate113.Reconstruction.Core
{
    public interface IRandomSource
    {
        int NextInt(int minimumInclusive, int maximumExclusive);

        float NextFloat01();
    }
}
