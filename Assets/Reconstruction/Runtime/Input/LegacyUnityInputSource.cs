using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodPirate113.Reconstruction.Input
{
    public sealed class LegacyUnityInputSource : MonoBehaviour, IGoodPirateInputSource
    {
        private const int MousePointerId = -1;

        [SerializeField, Min(0f)] private float tapMaximumDuration = 0.35f;
        [SerializeField, Min(0f)] private float tapMaximumMovement = 24f;
        [SerializeField, Min(0f)] private float doubleTapMaximumDelay = 0.3f;

        private readonly Dictionary<int, PointerTracker> pointers = new Dictionary<int, PointerTracker>();
        private bool inputEnabled = true;
        private bool pinchActive;
        private float previousPinchDistance;
        private double previousTapTime = double.NegativeInfinity;
        private Vector2 previousTapPosition;

        public event Action<GoodPiratePointerSample> PointerSampled;
        public event Action<GoodPirateTapSample> TapSampled;
        public event Action<GoodPiratePinchSample> PinchSampled;
        public event Action<GoodPirateWheelSample> WheelSampled;

        public bool IsEnabled
        {
            get => inputEnabled;
            set
            {
                if (inputEnabled == value)
                {
                    return;
                }

                inputEnabled = value;
                if (!inputEnabled)
                {
                    ReleaseAll();
                }
            }
        }

        private void Update()
        {
            ProcessFrame();
        }

        private void OnDisable()
        {
            ReleaseAll();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                ReleaseAll();
            }
        }

        public void ProcessFrame()
        {
            if (!inputEnabled)
            {
                return;
            }

            var timestamp = Time.unscaledTimeAsDouble;
            if (UnityEngine.Input.touchCount > 0)
            {
                ProcessTouches(timestamp);
            }
            else
            {
                ProcessMouse(timestamp);
                ResetPinch();
            }

            var wheel = UnityEngine.Input.mouseScrollDelta;
            if (wheel.sqrMagnitude > 0f)
            {
                WheelSampled?.Invoke(new GoodPirateWheelSample(
                    wheel,
                    UnityEngine.Input.mousePosition,
                    timestamp));
            }
        }

        public void ReleaseAll()
        {
            if (pointers.Count > 0)
            {
                var timestamp = Time.unscaledTimeAsDouble;
                foreach (var pair in pointers)
                {
                    PointerSampled?.Invoke(new GoodPiratePointerSample(
                        pair.Key,
                        GoodPiratePointerPhase.Cancelled,
                        pair.Value.LastPosition,
                        Vector2.zero,
                        0f,
                        timestamp,
                        pair.Value.IsPrimary));
                }

                pointers.Clear();
            }

            ResetPinch();
        }

        private void ProcessTouches(double timestamp)
        {
            for (var index = 0; index < UnityEngine.Input.touchCount; index++)
            {
                var touch = UnityEngine.Input.GetTouch(index);
                var pressure = touch.maximumPossiblePressure > 0f
                    ? touch.pressure / touch.maximumPossiblePressure
                    : 1f;
                var isPrimary = index == 0;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        BeginPointer(touch.fingerId, touch.position, pressure, timestamp, isPrimary);
                        break;
                    case TouchPhase.Moved:
                        MovePointer(touch.fingerId, touch.position, pressure, timestamp, isPrimary, false);
                        break;
                    case TouchPhase.Stationary:
                        MovePointer(touch.fingerId, touch.position, pressure, timestamp, isPrimary, true);
                        break;
                    case TouchPhase.Ended:
                        EndPointer(touch.fingerId, touch.position, pressure, timestamp, isPrimary, false, touch.tapCount);
                        break;
                    case TouchPhase.Canceled:
                        EndPointer(touch.fingerId, touch.position, pressure, timestamp, isPrimary, true, 0);
                        break;
                }
            }

            ProcessPinch(timestamp);
        }

        private void ProcessMouse(double timestamp)
        {
            Vector2 position = UnityEngine.Input.mousePosition;
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                BeginPointer(MousePointerId, position, 1f, timestamp, true);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                EndPointer(MousePointerId, position, 0f, timestamp, true, false, 0);
            }
            else if (UnityEngine.Input.GetMouseButton(0))
            {
                MovePointer(MousePointerId, position, 1f, timestamp, true, false);
            }
        }

        private void BeginPointer(
            int pointerId,
            Vector2 position,
            float pressure,
            double timestamp,
            bool isPrimary)
        {
            pointers[pointerId] = new PointerTracker(position, timestamp, isPrimary);
            PointerSampled?.Invoke(new GoodPiratePointerSample(
                pointerId,
                GoodPiratePointerPhase.Began,
                position,
                Vector2.zero,
                pressure,
                timestamp,
                isPrimary));
        }

        private void MovePointer(
            int pointerId,
            Vector2 position,
            float pressure,
            double timestamp,
            bool isPrimary,
            bool stationary)
        {
            if (!pointers.TryGetValue(pointerId, out var tracker))
            {
                BeginPointer(pointerId, position, pressure, timestamp, isPrimary);
                return;
            }

            var delta = position - tracker.LastPosition;
            tracker.LastPosition = position;
            tracker.MaximumSqrDistance = Mathf.Max(
                tracker.MaximumSqrDistance,
                (position - tracker.StartPosition).sqrMagnitude);
            pointers[pointerId] = tracker;

            PointerSampled?.Invoke(new GoodPiratePointerSample(
                pointerId,
                stationary ? GoodPiratePointerPhase.Held : GoodPiratePointerPhase.Moved,
                position,
                delta,
                pressure,
                timestamp,
                isPrimary));
        }

        private void EndPointer(
            int pointerId,
            Vector2 position,
            float pressure,
            double timestamp,
            bool isPrimary,
            bool cancelled,
            int platformTapCount)
        {
            if (!pointers.TryGetValue(pointerId, out var tracker))
            {
                tracker = new PointerTracker(position, timestamp, isPrimary);
            }

            var delta = position - tracker.LastPosition;
            tracker.MaximumSqrDistance = Mathf.Max(
                tracker.MaximumSqrDistance,
                (position - tracker.StartPosition).sqrMagnitude);

            PointerSampled?.Invoke(new GoodPiratePointerSample(
                pointerId,
                cancelled ? GoodPiratePointerPhase.Cancelled : GoodPiratePointerPhase.Ended,
                position,
                delta,
                pressure,
                timestamp,
                isPrimary));

            pointers.Remove(pointerId);

            if (!cancelled
                && timestamp - tracker.StartTime <= tapMaximumDuration
                && tracker.MaximumSqrDistance <= tapMaximumMovement * tapMaximumMovement)
            {
                EmitTap(position, timestamp, platformTapCount);
            }
        }

        private void EmitTap(Vector2 position, double timestamp, int platformTapCount)
        {
            var closeInTime = timestamp - previousTapTime <= doubleTapMaximumDelay;
            var closeInSpace = (position - previousTapPosition).sqrMagnitude
                <= tapMaximumMovement * tapMaximumMovement;
            var inferredTapCount = closeInTime && closeInSpace ? 2 : 1;
            var tapCount = platformTapCount > 0 ? platformTapCount : inferredTapCount;

            previousTapTime = timestamp;
            previousTapPosition = position;
            TapSampled?.Invoke(new GoodPirateTapSample(position, tapCount, timestamp));
        }

        private void ProcessPinch(double timestamp)
        {
            if (UnityEngine.Input.touchCount < 2)
            {
                ResetPinch();
                return;
            }

            var first = UnityEngine.Input.GetTouch(0).position;
            var second = UnityEngine.Input.GetTouch(1).position;
            var distance = Vector2.Distance(first, second);
            var delta = pinchActive ? distance - previousPinchDistance : 0f;
            previousPinchDistance = distance;
            pinchActive = true;

            PinchSampled?.Invoke(new GoodPiratePinchSample(
                (first + second) * 0.5f,
                distance,
                delta,
                timestamp));
        }

        private void ResetPinch()
        {
            pinchActive = false;
            previousPinchDistance = 0f;
        }

        private struct PointerTracker
        {
            public PointerTracker(Vector2 position, double startTime, bool isPrimary)
            {
                StartPosition = position;
                LastPosition = position;
                StartTime = startTime;
                MaximumSqrDistance = 0f;
                IsPrimary = isPrimary;
            }

            public Vector2 StartPosition;
            public Vector2 LastPosition;
            public double StartTime;
            public float MaximumSqrDistance;
            public bool IsPrimary;
        }
    }
}
