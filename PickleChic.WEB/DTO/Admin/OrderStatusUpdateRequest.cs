namespace PickleChic.WEB.DTO.Admin
{
    public class OrderStatusUpdateRequest
    {
        public string PaymentStatus { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;

        public string? UpdateBy { get; set; }
    }
}
