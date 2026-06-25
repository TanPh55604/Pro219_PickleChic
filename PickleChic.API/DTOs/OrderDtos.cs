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
