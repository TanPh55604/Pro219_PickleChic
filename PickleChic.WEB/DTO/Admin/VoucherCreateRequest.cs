namespace PickleChic.WEB.DTO.Admin
{
    public class VoucherCreateRequest
    {
        public string VoucherCode { get; set; } = string.Empty;

        public string DiscountType { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }

        public decimal MinOrderValue { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public decimal? MinimumSpend { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; }

        public int CustomerUsageLimit { get; set; }

        public int UsedCount { get; set; }

        public bool IsActive { get; set; }
    }
}
