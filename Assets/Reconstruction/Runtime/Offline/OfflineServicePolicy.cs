using System;

namespace GoodPirate113.Reconstruction.Offline
{
    public enum OfflineServiceDisposition
    {
        NoOp,
        Unavailable,
        LocalReward
    }

    public readonly struct OfflineServiceOperation
    {
        public OfflineServiceOperation(string service, string operation, OfflineServiceDisposition disposition)
        {
            Service = service ?? string.Empty;
            Operation = operation ?? string.Empty;
            Disposition = disposition;
        }

        public string Service { get; }

        public string Operation { get; }

        public OfflineServiceDisposition Disposition { get; }
    }

    public static class OfflineServicePolicy
    {
        public static event Action<OfflineServiceOperation> OperationHandled;

        public static void NoOp(string service, string operation)
        {
            Publish(service, operation, OfflineServiceDisposition.NoOp);
        }

        public static void Unavailable(string service, string operation)
        {
            Publish(service, operation, OfflineServiceDisposition.Unavailable);
        }

        public static void LocalReward(string service, string operation)
        {
            Publish(service, operation, OfflineServiceDisposition.LocalReward);
        }

        private static void Publish(string service, string operation, OfflineServiceDisposition disposition)
        {
            OperationHandled?.Invoke(new OfflineServiceOperation(service, operation, disposition));
        }
    }
}
