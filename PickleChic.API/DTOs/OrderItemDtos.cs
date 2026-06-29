namespace PickleChic.API.DTOs;

public class OrderItemCreateDto
{
    public int OrderId { get; set; }
    public int ProductVariantId { get; set; }
    public int? PromotionId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Subtotal { get; set; }
    public bool IsReviewed { get; set; }
    public string? UpdateBy { get; set; }
}

public class OrderItemUpdateDto : OrderItemCreateDto
{
    public int Id { get; set; }
}
