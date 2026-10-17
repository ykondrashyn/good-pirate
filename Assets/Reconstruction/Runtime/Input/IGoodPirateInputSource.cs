using System;

namespace GoodPirate113.Reconstruction.Input
{
    public interface IGoodPirateInputSource
    {
        event Action<GoodPiratePointerSample> PointerSampled;
        event Action<GoodPirateTapSample> TapSampled;
        event Action<GoodPiratePinchSample> PinchSampled;
        event Action<GoodPirateWheelSample> WheelSampled;

        bool IsEnabled { get; set; }

        void ProcessFrame();

        void ReleaseAll();
    }
}
