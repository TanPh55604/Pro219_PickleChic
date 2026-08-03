namespace PickleChic.WEB.DTO.Admin
{
    public class OrderStatusUpdateRequest
    {
        public string PaymentStatus { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;

        public string? UpdateBy { get; set; }

        public string? Reasons { get; set; }

        // TODO: hoàn trả số lượng tồn kho khi hủy đơn (chưa làm)
        // public bool? RestoreStock { get; set; }
    }
}
