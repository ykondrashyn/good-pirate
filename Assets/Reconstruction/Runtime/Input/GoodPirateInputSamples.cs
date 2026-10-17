using UnityEngine;

namespace GoodPirate113.Reconstruction.Input
{
    public enum GoodPiratePointerPhase
    {
        Began,
        Held,
        Moved,
        Ended,
        Cancelled
    }

    public readonly struct GoodPiratePointerSample
    {
        public GoodPiratePointerSample(
            int pointerId,
            GoodPiratePointerPhase phase,
            Vector2 screenPosition,
            Vector2 screenDelta,
            float pressure01,
            double timestamp,
            bool isPrimary)
        {
            PointerId = pointerId;
            Phase = phase;
            ScreenPosition = screenPosition;
            ScreenDelta = screenDelta;
            Pressure01 = Mathf.Clamp01(pressure01);
            Timestamp = timestamp;
            IsPrimary = isPrimary;
        }

        public int PointerId { get; }
        public GoodPiratePointerPhase Phase { get; }
        public Vector2 ScreenPosition { get; }
        public Vector2 ScreenDelta { get; }
        public float Pressure01 { get; }
        public double Timestamp { get; }
        public bool IsPrimary { get; }
    }

    public readonly struct GoodPirateTapSample
    {
        public GoodPirateTapSample(Vector2 screenPosition, int tapCount, double timestamp)
        {
            ScreenPosition = screenPosition;
            TapCount = Mathf.Max(1, tapCount);
            Timestamp = timestamp;
        }

        public Vector2 ScreenPosition { get; }
        public int TapCount { get; }
        public double Timestamp { get; }
    }

    public readonly struct GoodPiratePinchSample
    {
        public GoodPiratePinchSample(
            Vector2 screenCenter,
            float distance,
            float distanceDelta,
            double timestamp)
        {
            ScreenCenter = screenCenter;
            Distance = Mathf.Max(0f, distance);
            DistanceDelta = distanceDelta;
            Timestamp = timestamp;
        }

        public Vector2 ScreenCenter { get; }
        public float Distance { get; }
        public float DistanceDelta { get; }
        public double Timestamp { get; }
    }

    public readonly struct GoodPirateWheelSample
    {
        public GoodPirateWheelSample(Vector2 delta, Vector2 screenPosition, double timestamp)
        {
            Delta = delta;
            ScreenPosition = screenPosition;
            Timestamp = timestamp;
        }

        public Vector2 Delta { get; }
        public Vector2 ScreenPosition { get; }
        public double Timestamp { get; }
    }
}
