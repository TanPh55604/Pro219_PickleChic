namespace PickleChic.WEB.Model
{
    public class VoucherModel
    {
        public int Id { get; set; }

        public string VoucherCode { get; set; } = string.Empty;

        public string DiscountType { get; set; } = Constant.VoucherDiscountType.Percent;

        public decimal DiscountValue { get; set; }

        public decimal MinOrderValue { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public decimal? MinimumSpend { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int UsageLimit { get; set; } = 1;

        public int CustomerUsageLimit { get; set; } = 1;

        public int UsedCount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? OriginalStartDate { get; set; }

        public bool IsPercentDiscount => DiscountType == Constant.VoucherDiscountType.Percent;
    }
}
