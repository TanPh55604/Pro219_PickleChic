namespace PickleChic.WEB.DTO.Customer
{
    public class CartItemResponse
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public DateTime InsertedAt { get; set; }

        public CartProductVariantResponse? ProductVariant { get; set; }
    }

    public class CartProductVariantResponse
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string? VariantName { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int Status { get; set; }

        public string ProductName { get; set; } = string.Empty;
    }

    public class CartItemCreateRequest
    {
        public int CustomerId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }
    }

    public class CartItemUpdateRequest
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }
    }
}
