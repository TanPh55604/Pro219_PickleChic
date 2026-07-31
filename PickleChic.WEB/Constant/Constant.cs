namespace PickleChic.WEB.Constant
{
    public class Constant
    {
        public static string metaDataAdmin = "PickleChic Management -";
        public static string metaDataCustomer = "PickleChic Store -";

        public static class OrderStatus
        {
            public const string Pending = "Chờ xác nhận";
            public const string Processing = "Chờ xử lý";
            public const string WaitingForPayment = "Chờ thanh toán";
            public const string Confirmed = "Đã xác nhận";
            public const string Shipping = "Đang giao hàng";
            public const string ShippingDone = "Giao thành công";
            public const string DeliveryFailed = "Giao thất bại";
            public const string Cancelled = "Đã hủy(KH)";
            public const string Expired = "Đã hết hạn";
            public const string Done = "Hoàn thành";

            public const string StatusPending = Pending;
            public const string StatusConfirm = Confirmed;
            public const string StatusShipping = Shipping;
            public const string StatusShippingDone = ShippingDone;

            public static readonly string[] All =
            [
                Pending,
                Processing,
                WaitingForPayment,
                Confirmed,
                Shipping,
                ShippingDone,
                DeliveryFailed,
                Cancelled,
                Expired,
                Done
            ];

            public static readonly string[] TerminalStatuses =
            [
                Done,
                Cancelled,
                Expired,
                DeliveryFailed
            ];
        }

        public static class PaymentStatus
        {
            public const string Pending = "Chờ thanh toán";
            public const string Completed = "Đã thanh toán";
            public const string Cancelled = "Đã hủy";

            public static readonly string[] All =
            [
                Pending,
                Completed,
                Cancelled
            ];
        }

        public static class PaymentMethodType
        {
            public const int Cash = 1;
            public const int BankTransfer = 2;
        }

        public static class OfflinePos
        {
            public const string DraftStorageKey = "picklechic_offline_pos_drafts";

            public const int MaxTabs = 15;
        }
    }
}
