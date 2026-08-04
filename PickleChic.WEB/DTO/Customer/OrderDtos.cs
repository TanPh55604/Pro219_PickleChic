namespace PickleChic.WEB.DTO.Customer
{
    public class OrderCalculationRequest
    {
        public List<OrderItemCalculationRequest> Items { get; set; } = new();

        public string? DiscountCode { get; set; }

        public int? AddressId { get; set; }

        public bool? UsePoints { get; set; }

        public bool? Bopis { get; set; }
    }

    public class OrderItemCalculationRequest
    {
        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }
    }

    public class OrderCalculationResult
    {
        public decimal TotalAmount { get; set; }

        public decimal DiscountPrice { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal FinalAmount { get; set; }

        public int? VoucherId { get; set; }

        public List<OrderCalculationItemResult> Items { get; set; } = new();

        public decimal PointsDiscountPrice { get; set; }

        public int PointsDeducted { get; set; }
    }

    public class OrderCalculationItemResult
    {
        public int ProductVariantId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string VariantName { get; set; } = string.Empty;

        public string AttributeName { get; set; } = string.Empty;

        public string AttributeValue { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal ListedPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal PriceToPay { get; set; }
    }

    public class CheckoutRequest
    {
        public List<CartItemCheckoutRequest> ListItemCheckout { get; set; } = new();

        public AddressCreateRequest? AddressDTO { get; set; }
    }

    public class CartItemCheckoutRequest
    {
        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public string ProductName { get; set; } = string.Empty;
    }

    public class CheckoutResponse
    {
        public string OrderCode { get; set; } = string.Empty;

        public int OrderId { get; set; }

        public int? PaymentType { get; set; }

        public string? URLPayment { get; set; }
    }

    public class PaymentCallbackOrderResponse
    {
        public int Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;
    }

    public class UserOrderDetailResponse
    {
        public int Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal ShippingFee { get; set; }

        public string Notes { get; set; } = string.Empty;

        public string PurchaseChannel { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;

        public int? Status { get; set; }

        public string? PaymentLink { get; set; }

        public DateTime? PaymentExpiration { get; set; }

        public bool IsOrderPOS { get; set; }

        public bool? BOPIS { get; set; }

        public string? CustomerType { get; set; }

        public string ReceiverName { get; set; } = string.Empty;

        public string ReceiverPhone { get; set; } = string.Empty;

        public string FullAddress { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalPrice { get; set; }

        public List<UserOrderItemDetailResponse> OrderItems { get; set; } = new();

        public string? StatusHistory { get; set; }
    }

    public class UserOrderItemDetailResponse
    {
        public int ProductVariantId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string VariantName { get; set; } = string.Empty;

        public List<string> Attributes { get; set; } = new();

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal Subtotal { get; set; }
    }

    public class CancelOrderRequest
    {
        public string CancelReason { get; set; } = string.Empty;

        public string? CancelDetail { get; set; }
    }

    public class CancelOrderDialogResult
    {
        public string CancelReason { get; set; } = string.Empty;

        public string? CancelDetail { get; set; }

        // TODO: hoàn trả số lượng tồn kho khi hủy đơn (chưa làm)
        // public bool RestoreStock { get; set; }
    }
}
