using System;
using System.Collections.Generic;

namespace GoodPirate113.Reconstruction.Modal
{
    public sealed class ModalCoordinator : IModalCoordinator
    {
        private readonly object sync = new object();
        private readonly Dictionary<long, string> activeModals = new Dictionary<long, string>();
        private long nextLeaseId;

        public event Action<bool> ModalStateChanged;

        public bool IsModalOpen
        {
            get
            {
                lock (sync)
                {
                    return activeModals.Count > 0;
                }
            }
        }

        public int ActiveModalCount
        {
            get
            {
                lock (sync)
                {
                    return activeModals.Count;
                }
            }
        }

        public IDisposable Acquire(string modalId)
        {
            if (string.IsNullOrWhiteSpace(modalId))
            {
                throw new ArgumentException("A modal identifier is required.", nameof(modalId));
            }

            long leaseId;
            bool becameOpen;
            lock (sync)
            {
                if (nextLeaseId == long.MaxValue)
                {
                    throw new InvalidOperationException("No more modal lease identifiers are available.");
                }

                leaseId = ++nextLeaseId;
                becameOpen = activeModals.Count == 0;
                activeModals.Add(leaseId, modalId);
            }

            if (becameOpen)
            {
                ModalStateChanged?.Invoke(true);
            }

            return new ModalLease(this, leaseId);
        }

        private void Release(long leaseId)
        {
            bool becameClosed;
            lock (sync)
            {
                if (!activeModals.Remove(leaseId))
                {
                    return;
                }

                becameClosed = activeModals.Count == 0;
            }

            if (becameClosed)
            {
                ModalStateChanged?.Invoke(false);
            }
        }

        private sealed class ModalLease : IDisposable
        {
            private ModalCoordinator owner;
            private readonly long leaseId;

            public ModalLease(ModalCoordinator owner, long leaseId)
            {
                this.owner = owner;
                this.leaseId = leaseId;
            }

            public void Dispose()
            {
                var coordinator = owner;
                owner = null;
                coordinator?.Release(leaseId);
            }
        }
    }
}
