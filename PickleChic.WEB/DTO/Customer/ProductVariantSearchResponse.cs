using PickleChic.WEB.DTO.Admin;

namespace PickleChic.WEB.DTO.Customer
{
    public class ProductVariantSearchResponse
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string? VariantName { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int Status { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ProductDescription { get; set; }

        public string? CategoryName { get; set; }

        public string? BrandName { get; set; }

        public List<ProductVariantImageResponse> Images { get; set; } = new();

        public List<ProductVariantAttributeResponse> Attributes { get; set; } = new();

        public string? MainImageUrl =>
            Images.FirstOrDefault(i => i.IsMain)?.URL
            ?? Images.FirstOrDefault()?.URL;

        public string DisplayName =>
            string.IsNullOrWhiteSpace(VariantName)
                ? ProductName
                : $"{ProductName} - {VariantName}";

        public bool InStock => StockQuantity > 0 && Status != -1;
    }
}
