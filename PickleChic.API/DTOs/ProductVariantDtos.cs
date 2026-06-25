namespace PickleChic.API.DTOs;

public class ProductVariantCreateDto
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = null!;
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Status { get; set; }
}

public class ProductVariantUpdateDto : ProductVariantCreateDto
{
    public int Id { get; set; }
}

public class ProductVariantWithAttributesCreateDto
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = null!;
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Status { get; set; }
    public List<int> AttributeValueIds { get; set; } = new();
}

public class ProductVariantWithAttributesUpdateDto : ProductVariantWithAttributesCreateDto
{
    public int Id { get; set; }
}

public class ProductVariantResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = null!;
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Status { get; set; }
    public List<int> AttributeValueIds { get; set; } = new();
}

public class ProductVariantFilterDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = null!;
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Status { get; set; }
    public string ProductName { get; set; } = null!;
    public string? ProductDescription { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryDescription { get; set; }
    public string? BrandName { get; set; }
    public string? BrandDescription { get; set; }
    public List<ProductVariantImageDetailDto> Images { get; set; } = new();
    public List<AttributeValueDetailDto> Attributes { get; set; } = new();
}

public class ProductVariantSearchResultDto : ProductVariantFilterDto
{
}


