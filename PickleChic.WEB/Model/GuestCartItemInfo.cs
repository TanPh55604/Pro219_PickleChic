namespace PickleChic.WEB.Model
{
    public class GuestCartItemInfo
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string VariantName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }
    }
}
