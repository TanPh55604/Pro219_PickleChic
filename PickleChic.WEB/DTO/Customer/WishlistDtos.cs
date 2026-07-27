using PickleChic.WEB.DTO.Admin;

namespace PickleChic.WEB.DTO.Customer
{
    public class WishlistCreateRequest
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
    }

    public class WishlistItemResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
    }

    public class WishlistProductResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public int Status { get; set; }
        public List<WishlistProductVariantResponse> ProductVariants { get; set; } = new();
    }

    public class WishlistProductVariantResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? VariantName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int Status { get; set; }
        public string? ProductName { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public List<ProductVariantImageResponse> Images { get; set; } = new();
    }
}
