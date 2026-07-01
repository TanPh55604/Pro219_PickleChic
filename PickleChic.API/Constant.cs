namespace PickleChic.API;

public static class Constant
{
    public static class OrderStatus
    {
        public const string Pending = "Chờ xác nhận";
        public const string Processing = "Chờ xử lý";
        public const string WaitingForPayment = "Chờ thanh toán";
        public const string Confirmed = "Đã xác nhận";
        public const string Cancelled = "Đã hủy(KH)";
        public const string Expired = "Đã hết hạn";
        public const string Done = "Hoàn thành";
    }

    public static class PaymentStatus
    {
        public const string Pending = "Chờ thanh toán";
        public const string Completed = "Đã thanh toán";
        public const string Cancelled = "Đã hủy";
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
