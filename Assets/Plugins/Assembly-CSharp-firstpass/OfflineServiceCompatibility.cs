// Callback-only adapters for retired integrations. No native or network bridges.
using System;

namespace UnityEngine.Purchasing
{
    public enum InitializationFailureReason { PurchasingUnavailable, NoProductsAvailable, AppNotKnown }
    public enum PurchaseFailureReason { PurchasingUnavailable, ExistingPurchasePending, ProductUnavailable, SignatureInvalid, UserCancelled, PaymentDeclined, DuplicateTransaction, Unknown }
    public enum PurchaseProcessingResult { Complete, Pending }
    public interface IStoreController { }
    public interface IExtensionProvider { }
    public interface IStoreListener
    {
        void OnInitialized(IStoreController controller, IExtensionProvider extensions);
        void OnInitializeFailed(InitializationFailureReason reason);
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args);
        void OnPurchaseFailed(Product product, PurchaseFailureReason reason);
    }
    public sealed class ProductDefinition { public string id; public ProductDefinition(string id) { this.id = id; } }
    public sealed class Product { public ProductDefinition definition; public string receipt = ""; public string transactionID = ""; }
    public sealed class PurchaseEventArgs : EventArgs { public Product purchasedProduct; public PurchaseEventArgs(Product product) { purchasedProduct = product; } }
}
namespace GooglePlayGames.BasicApi.SavedGame
{
    public enum SavedGameRequestStatus { Success, InternalError, AuthenticationError }
    public interface ISavedGameMetadata { }
}
namespace SA.Foundation.Templates
{
    public sealed class SA_Error { public string FullMessage => "This service is unavailable in the offline reconstruction"; }
    public sealed class SA_Result { public bool IsSucceeded => false; public SA_Error Error => new SA_Error(); }
}
namespace SA.iOS.Foundation
{
    public sealed class ISN_NSKeyValueObject { public string StringValue => ""; public bool BoolValue => false; public int IntValue => 0; }
    public static class ISN_NSUbiquitousKeyValueStore
    {
        public static ISN_NSKeyValueObject KeyValueStoreObjectForKey(string key) => null;
        public static void SetString(string key, string value) { }
        public static void SetBool(string key, bool value) { }
        public static void SetInt(string key, int value) { }
        public static void Synchronize() { }
    }
}
namespace SA.CrossPlatform.Notifications
{
    public interface UM_iNotificationsClient
    {
        void RemoveAllDeliveredNotifications();
        void AddNotificationRequest(UM_NotificationRequest request, Action<SA.Foundation.Templates.SA_Result> callback);
    }
    public static class UM_NotificationCenter
    {
        private sealed class OfflineClient : UM_iNotificationsClient
        {
            public void RemoveAllDeliveredNotifications() { }
            public void AddNotificationRequest(UM_NotificationRequest request, Action<SA.Foundation.Templates.SA_Result> callback) => callback?.Invoke(new SA.Foundation.Templates.SA_Result());
        }
        public static UM_iNotificationsClient Client { get; } = new OfflineClient();
    }
    public sealed class UM_Notification { public void SetTitle(string title) { } public void SetBody(string body) { } }
    public sealed class UM_TimeIntervalNotificationTrigger { public UM_TimeIntervalNotificationTrigger(int seconds) { } }
    public sealed class UM_NotificationRequest { public UM_NotificationRequest(int id, UM_Notification notification, UM_TimeIntervalNotificationTrigger trigger) { } }
}
