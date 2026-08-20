namespace PickleChic.API.DTOs;

public class VoucherCreateDto
{
    public bool IsForever { get; set; } = false;
    public string VoucherCode { get; set; } = null!;
    public string DiscountType { get; set; } = null!;
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
    public int? RankId { get; set; } = null;
}

public class VoucherUpdateDto : VoucherCreateDto
{
    public int Id { get; set; }
}
