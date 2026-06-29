namespace PickleChic.API;

public static class Constant
{
    public static class OrderStatus
    {
        public const string StatusPending = "Pending";
        public const string OrderStatusPending = "Pending";
        public const string PaymentPending = "Pending";
        
        public const string StatusWaitingForPayment = "WaitingForPayment";
        public const string OrderStatusWaitingForPayment = "WaitingForPayment";
        
        public const string PaymentCompleted = "Completed";
        public const string OrderStatusConfirm = "Confirmed";
        public const string StatusConfirm = "Confirmed";
        
        public const string StatusCanceledByUser = "Cancelled";
        public const string PaymentCancelled = "Cancelled";
        public const string OrderStatusCanceledByUser = "Cancelled";
        public const string OrderStatusPaymentExpired = "Expired";
        
        public const string StatusDone = "Done";
        public const string OrderStatusDone = "Done";
    }

    public static class CustomerType
    {
        public const string GuestOrder = "Guest";
        public const string RegisteredOrder = "Registered";
    }

    public static class ErrorCode
    {
        public const string OtherError = "OtherError";
        public const string DatabaseError = "DatabaseError";
    }
}
