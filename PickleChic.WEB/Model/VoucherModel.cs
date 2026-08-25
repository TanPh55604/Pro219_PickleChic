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

        public int? RankId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? UsageLimit { get; set; } = 100;

        public int CustomerUsageLimit { get; set; } = 1;

        public int UsedCount { get; set; }

        public bool IsActive { get; set; } = false;

        public bool IsForever { get; set; }

        public DateTime? OriginalStartDate { get; set; }

        public bool IsPercentDiscount => DiscountType == Constant.VoucherDiscountType.Percent;

        public static bool IsForeverEndDate(DateTime endDate) =>
            endDate.Year >= 9999 || endDate == DateTime.MaxValue;

        public static bool IsUnlimitedUsage(int usageLimit) =>
            usageLimit <= 0 || usageLimit == int.MaxValue;
    }
}
