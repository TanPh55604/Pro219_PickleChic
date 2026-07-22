using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;

namespace PickleChic.WEB.Model
{
    public class CartLineModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int ProductVariantId { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Variant { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int StockQuantity { get; set; }

        public string ProductUrl { get; set; } = string.Empty;

        public decimal LineTotal => Price * Quantity;

        public bool CanIncrease => Quantity < StockQuantity;

        public static CartLineModel FromResponse(CartItemResponse item)
        {
            var variant = item.ProductVariant;
            var productId = variant?.ProductId ?? 0;
            var productName = variant?.ProductName ?? string.Empty;
            var variantName = variant?.VariantName;

            return new CartLineModel
            {
                Id = item.Id,
                CustomerId = item.CustomerId,
                ProductVariantId = item.ProductVariantId,
                ProductId = productId,
                Name = productName,
                Variant = string.IsNullOrWhiteSpace(variantName)
                    ? variant?.SKU ?? string.Empty
                    : variantName,
                Price = variant?.Price ?? 0,
                Quantity = item.Quantity,
                StockQuantity = variant?.StockQuantity ?? 0,
                ProductUrl = productId > 0
                    ? RouterConfig.BuildRoute(RouterConfig.Customer.ProductDetail, productId)
                    : RouterConfig.Customer.Products
            };
        }
    }
}
