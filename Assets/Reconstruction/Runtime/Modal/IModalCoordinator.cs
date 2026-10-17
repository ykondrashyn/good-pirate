using System;

namespace GoodPirate113.Reconstruction.Modal
{
    public interface IModalCoordinator
    {
        event Action<bool> ModalStateChanged;

        bool IsModalOpen { get; }

        int ActiveModalCount { get; }

        IDisposable Acquire(string modalId);
    }
}
