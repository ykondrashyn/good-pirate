using UnityEngine;

namespace GoodPirate113.Reconstruction.Platform
{
    public static class GoodPirateFrameRatePolicy
    {
        public const int BrowserManagedFrameRate = -1;

        public static int ResolveTargetFrameRate(
            int recoveredTargetFrameRate,
            bool browserOwnsPresentationTiming)
        {
            return browserOwnsPresentationTiming
                ? BrowserManagedFrameRate
                : recoveredTargetFrameRate;
        }

        public static void ApplyRecoveredTargetFrameRate(
            int recoveredTargetFrameRate)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Application.targetFrameRate = ResolveTargetFrameRate(
                recoveredTargetFrameRate,
                true);
#else
            Application.targetFrameRate = ResolveTargetFrameRate(
                recoveredTargetFrameRate,
                false);
#endif
        }
    }
}
