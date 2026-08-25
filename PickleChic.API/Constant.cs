namespace PickleChic.API;

public static class Constant
{
    public static class OrderStatus
    {
        public const string Pending = "Chờ xác nhận"; // status 1
        public const string Processing = "Chờ xử lý"; // status 2
        public const string WaitingForPayment = "Chờ thanh toán"; // status 3
        public const string Confirmed = "Đã xác nhận"; // status 4
        public const string Cancelled = "Đã hủy(KH)"; // status -1
        public const string Expired = "Đã hết hạn"; // status -2
        public const string AwaitingPickup = "Chờ khách hàng tới lấy"; // status 6
        public const string Shiping = "Đang giao hàng"; // status 5
        public const string Done = "Hoàn thành"; // status 7

        public static int? GetStatusInt(string? orderStatus) => orderStatus switch
        {
            Pending or "Pending" => 1,
            Processing or "Processing" => 2,
            WaitingForPayment or "WaitingForPayment" => 3,
            Confirmed or "Confirmed" => 4,
            Cancelled or "Cancelled" or "Đã hủy" or "Giao thất bại" => -1,
            Expired or "Expired" or "Đã hết hạn" or "Hết hạn thanh toán" => -2,
            AwaitingPickup or "AwaitingPickup" => 6,
            Shiping or "Shiping" or "Shipping" or "Đang giao hàng" => 5,
            Done or "Done" => 7,
            _ => null
        };
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

    public static class AddressStatus
    {
        public const int SystemPickup = 0;
        public const int Active = 1;
    }

    public static class ErrorCode
    {
        public const string OtherError = "OtherError";
        public const string DatabaseError = "DatabaseError";
    }
}
