using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;

namespace PickleChic.WEB.Model
{
    public class ProductCardModel
    {
        public int VariantId { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        public decimal? OldPrice { get; set; }

        public string? PriceDisplay { get; set; }

        public string? OldPriceDisplay { get; set; }

        public int DiscountPercent { get; set; }

        public string? Tag { get; set; }

        public string? BrandName { get; set; }

        public string? CategoryName { get; set; }

        public bool InStock { get; set; } = true;

        public int StockQuantity { get; set; }

        public string? DetailUrl { get; set; }

        public string DisplayPrice =>
            PriceDisplay ?? (Price > 0 ? $"{Price:N0}đ" : "—");

        public string? DisplayOldPrice =>
            OldPriceDisplay ?? (OldPrice.HasValue ? $"{OldPrice.Value:N0}đ" : null);

        public string ResolvedDetailUrl =>
            DetailUrl ?? (ProductId > 0
                ? RouterConfig.BuildRoute(RouterConfig.Customer.ProductDetail, ProductId)
                : RouterConfig.Customer.Search);

        public static ProductCardModel FromVariant(ProductVariantSearchResponse variant)
        {
            return new ProductCardModel
            {
                VariantId = variant.Id,
                ProductId = variant.ProductId,
                Name = variant.DisplayName,
                ImageUrl = variant.MainImageUrl,
                Price = variant.Price,
                BrandName = variant.BrandName,
                CategoryName = variant.CategoryName,
                InStock = variant.InStock,
                StockQuantity = variant.StockQuantity,
                Tag = variant.InStock ? null : "Hết hàng"
            };
        }

        public static ProductCardModel FromDisplay(
            string name,
            string imageUrl,
            string priceDisplay,
            string? oldPriceDisplay = null,
            int discountPercent = 0,
            string? tag = null,
            string? brandName = null,
            string? categoryName = null,
            int productId = 0,
            int variantId = 0,
            string? detailUrl = null,
            bool inStock = true)
        {
            return new ProductCardModel
            {
                VariantId = variantId,
                ProductId = productId,
                Name = name,
                ImageUrl = imageUrl,
                PriceDisplay = priceDisplay,
                OldPriceDisplay = oldPriceDisplay,
                DiscountPercent = discountPercent,
                Tag = tag,
                BrandName = brandName,
                CategoryName = categoryName,
                DetailUrl = detailUrl,
                InStock = inStock
            };
        }
    }
}
