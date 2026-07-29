namespace PickleChic.API.DTOs;

public class CartItemCreateDto
{
    public int CustomerId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}

public class CartItemUpdateDto : CartItemCreateDto
{
    public int Id { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public DateTime InsertedAt { get; set; }
    public CartProductVariantDto? ProductVariant { get; set; }
}

public class CartProductVariantDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = null!;
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Status { get; set; }
    public string ProductName { get; set; } = null!;
}

public class AddToCartPosDto
{
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}

public class AddToCartPosResultDto
{
    public bool AbleToAdd { get; set; }
    public string? ErrorMessage { get; set; }
}

