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
            public const string AwaitingPickup = "Chờ khách hàng tới lấy";
            public const string ShippingDone = "Giao thành công";
            public const string DeliveryFailed = "Giao thất bại";
            public const string Cancelled = "Đã hủy(KH)";
            public const string Expired = "Đã hết hạn";
            public const string Done = "Hoàn thành";

            public static class Code
            {
                public const int Pending = 1;
                public const int Processing = 2;
                public const int WaitingForPayment = 3;
                public const int Confirmed = 4;
                public const int Shipping = 5;
                public const int AwaitingPickup = 6;
                public const int Done = 7;
                public const int Cancelled = -1;
                public const int Expired = -2;
            }

            public static readonly int[] TerminalCodes =
            [
                Code.Done,
                Code.Cancelled,
                Code.Expired
            ];

            public static readonly string[] TerminalStatuses =
            [
                Done,
                Cancelled,
                Expired,
                DeliveryFailed
            ];

            public static string GetLabel(int code) => code switch
            {
                Code.Pending => Pending,
                Code.Processing => Processing,
                Code.WaitingForPayment => WaitingForPayment,
                Code.Confirmed => Confirmed,
                Code.Shipping => Shipping,
                Code.AwaitingPickup => AwaitingPickup,
                Code.Done => Done,
                Code.Cancelled => Cancelled,
                Code.Expired => Expired,
                _ => $"Trạng thái {code}"
            };

            public static int? ToCode(string? orderStatus) => orderStatus switch
            {
                Pending or "Pending" => Code.Pending,
                Processing or "Processing" => Code.Processing,
                WaitingForPayment or "WaitingForPayment" => Code.WaitingForPayment,
                Confirmed or "Confirmed" => Code.Confirmed,
                Cancelled or "Cancelled" or "Đã hủy" => Code.Cancelled,
                Expired or "Expired" or "Hết hạn thanh toán" => Code.Expired,
                AwaitingPickup or "AwaitingPickup" => Code.AwaitingPickup,
                Shipping or "Shiping" or "Shipping" => Code.Shipping,
                ShippingDone => Code.Done,
                DeliveryFailed => Code.Cancelled,
                Done or "Done" => Code.Done,
                _ => null
            };

            public static bool IsPendingGroup(int code) =>
                code is Code.Pending or Code.Processing or Code.WaitingForPayment;
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

        public static class OrderCancelReason
        {
            public const string Other = "Lý do khác";

            public static readonly string[] Customer =
            [
                "Muốn thay đổi sản phẩm / màu / size",
                "Tìm thấy giá rẻ hơn",
                "Thời gian giao hàng quá lâu",
                "Đặt nhầm / trùng đơn",
                "Không còn nhu cầu",
                "Không liên hệ được shop",
                Other
            ];

            public static readonly string[] Admin =
            [
                "Khách yêu cầu hủy",
                "Hết hàng / không đủ tồn",
                "Không liên hệ được khách",
                "Địa chỉ giao hàng không hợp lệ",
                "Đơn nghi ngờ gian lận",
                Other
            ];

            public static string Format(string reason, string? detail)
            {
                var selected = reason.Trim();
                if (string.IsNullOrWhiteSpace(detail))
                {
                    return selected;
                }

                return $"{selected}: {detail.Trim()}";
            }
        }
    }
}
