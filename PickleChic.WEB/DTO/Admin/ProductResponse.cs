namespace PickleChic.WEB.DTO.Admin
{
    public class ProductVariantAttributeResponse
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }
    }

    public class ProductVariantImageResponse
    {
        public int Id { get; set; }

        public string URL { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool IsMain { get; set; }
    }

    public class ProductVariantSummaryResponse
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string? VariantName { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int Status { get; set; }

        public List<ProductVariantImageResponse> Images { get; set; } = new();

        public List<ProductVariantAttributeResponse> Attributes { get; set; } = new();

        public bool IsActive => Status == 1;

        public string? MainImageUrl =>
            Images.FirstOrDefault(i => i.IsMain)?.URL
            ?? Images.FirstOrDefault()?.URL;

        public string AttributesDisplay =>
            Attributes.Count == 0
                ? "—"
                : string.Join(", ", Attributes.Select(a =>
                    string.IsNullOrWhiteSpace(a.AttributeName)
                        ? a.Value
                        : $"{a.AttributeName}: {a.Value}"));
    }

    public class ProductDetailResponse
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int BrandId { get; set; }

        public string? BrandName { get; set; }

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public List<ProductVariantSummaryResponse> ProductVariants { get; set; } = new();

        public bool IsActive => Status == 1;
    }

    public class ProductResponse
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public int BrandId { get; set; }

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
