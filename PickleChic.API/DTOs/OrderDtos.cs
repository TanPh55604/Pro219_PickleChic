namespace PickleChic.API.DTOs;

public class OrderCreateDto
{
    public int CustomerId { get; set; }
    public string OrderCode { get; set; } = null!;
    public int AddressId { get; set; }
    public DateTime OrderDate { get; set; }
    public int PaymentMethodId { get; set; }
    public int? VoucherId { get; set; }
    public string PaymentStatus { get; set; } = null!;
    public string OrderStatus { get; set; } = null!;
    public string? Notes { get; set; }
    public string? CustomerType { get; set; }
    public bool IsOrderPOS { get; set; }
    public string? PaymentLink { get; set; }
    public DateTime? PaymentExpiration { get; set; }
    public decimal ShippingFee { get; set; }
    public string? StatusHistory { get; set; }
    public string? UpdateBy { get; set; }
}

public class OrderUpdateDto : OrderCreateDto
{
    public int Id { get; set; }
    public DateTime? LastUpdate { get; set; }
}

public class CheckoutParamsDTO
{
    public List<CartItemCheckoutDTO> ListItemCheckout { get; set; } = null!;
    public AddressCreateDto? AddressDTO { get; set; }
}

public class CartItemCheckoutDTO
{
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; } = null!;
}

public class CheckoutDTO
{
    public string OrderCode { get; set; } = null!;
    public int OrderId { get; set; }
    public int? PaymentType { get; set; }
    public string? URLPayment { get; set; }
}

public class StatusHistoryEntry
{
    public int Index { get; set; }
    public string Status { get; set; } = null!;
    public string OrderStatus { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string DateTime { get; set; } = null!;
}

public class OrderStatusUpdateDto
{
    public string PaymentStatus { get; set; } = null!;
    public string OrderStatus { get; set; } = null!;
    public string? UpdateBy { get; set; }
}

public class OrderCalculationRequestDto
{
    public List<OrderItemCalculationDto> Items { get; set; } = null!;
    public string? DiscountCode { get; set; }
    public int? AddressId { get; set; }
}

public class OrderItemCalculationDto
{
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}

public class OrderCalculationResultDto
{
    public decimal TotalAmount { get; set; }
    public decimal DiscountPrice { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal FinalAmount { get; set; }
    public int? VoucherId { get; set; }
    public List<OrderCalculationItemResultDto> Items { get; set; } = null!;
}

public class OrderCalculationItemResultDto
{
    public int ProductVariantId { get; set; }
    public string ProductName { get; set; } = null!;
    public string VariantName { get; set; } = null!;
    public string AttributeName { get; set; } = null!;
    public string AttributeValue { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal ListedPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PriceToPay { get; set; }
}