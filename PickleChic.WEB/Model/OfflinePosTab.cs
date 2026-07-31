namespace PickleChic.WEB.Model
{
    public class OfflinePosTab
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "Hóa đơn";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OfflinePosLineItem> Items { get; set; } = new();

        public bool IsGuest { get; set; } = true;

        public int? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerEmail { get; set; }

        public int? AddressId { get; set; }

        public string? AddressLabel { get; set; }

        public OfflinePosGuestAddress? GuestAddress { get; set; }

        public int? VoucherId { get; set; }

        public string? VoucherCode { get; set; }

        public bool IsShipping { get; set; }

        public string PaymentMethod { get; set; } = "cash";

        public decimal? CashReceived { get; set; }

        public string? Note { get; set; }
    }

    public class OfflinePosLineItem
    {
        public int ProductVariantId { get; set; }

        public int Quantity { get; set; } = 1;

        public string? Sku { get; set; }

        public string? ProductName { get; set; }

        public string? VariantName { get; set; }

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public string DisplayName =>
            string.IsNullOrWhiteSpace(VariantName)
                ? (ProductName ?? $"#{ProductVariantId}")
                : $"{ProductName} - {VariantName}";
    }

    public class OfflinePosGuestAddress
    {
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string DetailInfo { get; set; } = string.Empty;

        public int WardId { get; set; }

        public int? ProvinceId { get; set; }

        public int? DistrictId { get; set; }
    }

    public class OfflinePosDraftStore
    {
        public List<OfflinePosTab> Tabs { get; set; } = new();

        public Guid ActiveTabId { get; set; }

        public int TabSequence { get; set; } = 1;
    }
}
