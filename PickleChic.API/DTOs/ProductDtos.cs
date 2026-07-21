namespace PickleChic.API.DTOs;

public class ProductCreateDto
{
    public string ProductName { get; set; } = null!;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public int Status { get; set; }
    public string? UpdatedBy { get; set; }
}

public class ProductUpdateDto : ProductCreateDto
{
    public int Id { get; set; }
}

public class ProductDetailDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int BrandId { get; set; }
    public string? BrandName { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public List<ProductVariantDetailDto> ProductVariants { get; set; } = new();
}

public class ProductVariantDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = null!;
    public string? VariantName { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Status { get; set; }
    public List<ProductVariantImageDetailDto> Images { get; set; } = new();
    public List<AttributeValueDetailDto> Attributes { get; set; } = new();
}

public class ProductVariantImageDetailDto
{
    public int Id { get; set; }
    public string URL { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsMain { get; set; }
}

public class AttributeValueDetailDto
{
    public int Id { get; set; }
    public int AttributeId { get; set; }
    public string AttributeName { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Note { get; set; }
}

public class ProductSearchResultDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int BrandId { get; set; }
    public string? BrandName { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public List<ProductVariantFilterDto> ProductVariants { get; set; } = new();
}

public class ProductFilterRequestDto
{
    public string? Keyword { get; set; }
    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }
    public int? AttributeId { get; set; }
    public List<int>? AttributeValueIds { get; set; }
    public bool IncludeInactiveVariants { get; set; } = false;
}

public class ProductFilterResponseDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int BrandId { get; set; }
    public string? BrandName { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public List<ProductVariantDetailDto> ProductVariants { get; set; } = new();
}


