namespace PickleChic.WEB.DTO.Admin
{
    public class ProductVariantUpdateRequest
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string? VariantName { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int Status { get; set; }

        public List<int> AttributeValueIds { get; set; } = new();
    }
}
