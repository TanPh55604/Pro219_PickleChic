namespace PickleChic.WEB.DTO.Admin
{
    public class OrderResponse
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public int AddressId { get; set; }

        public DateTime OrderDate { get; set; }

        public int PaymentMethodId { get; set; }

        public int? VoucherId { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;

        public int? Status { get; set; }

        public string? Notes { get; set; }

        public DateTime? LastUpdate { get; set; }

        public string? CustomerType { get; set; }

        public bool IsOrderPOS { get; set; }

        public bool? BOPIS { get; set; }

        public string? PaymentLink { get; set; }

        public DateTime? PaymentExpiration { get; set; }

        public decimal ShippingFee { get; set; }

        public string? StatusHistory { get; set; }

        public string? UpdateBy { get; set; }

        public DateTime InsertedAt { get; set; }

        public OrderCustomerResponse? Customer { get; set; }

        public OrderAddressResponse? Address { get; set; }

        public OrderPaymentMethodResponse? PaymentMethod { get; set; }

        public OrderVoucherResponse? Voucher { get; set; }

        public List<OrderItemResponse> OrderItems { get; set; } = new();

        public decimal ItemsTotal => OrderItems.Sum(i => i.Subtotal);

        public decimal GrandTotal => ItemsTotal + ShippingFee;
    }

    public class OrderCustomerResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }

    public class OrderAddressResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string DetailInfo { get; set; } = string.Empty;

        public OrderWardResponse? Ward { get; set; }
    }

    public class OrderWardResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public OrderDistrictResponse? District { get; set; }
    }

    public class OrderDistrictResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public OrderProvinceResponse? Province { get; set; }
    }

    public class OrderProvinceResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class OrderPaymentMethodResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class OrderVoucherResponse
    {
        public int Id { get; set; }

        public string VoucherCode { get; set; } = string.Empty;

        public string DiscountType { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }
    }

    public class OrderItemResponse
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal Subtotal { get; set; }

        public OrderProductVariantResponse? ProductVariant { get; set; }
    }

    public class OrderProductVariantResponse
    {
        public int Id { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string? VariantName { get; set; }

        public OrderProductResponse? Product { get; set; }
    }

    public class OrderProductResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class StatusHistoryEntryResponse
    {
        public int Index { get; set; }

        public string Status { get; set; } = string.Empty;

        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string DateTime { get; set; } = string.Empty;
    }
}
