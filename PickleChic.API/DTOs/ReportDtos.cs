namespace PickleChic.API.DTOs;

public class StatisticsQueryDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? GroupBy { get; set; } = "month";
}

public class TopProductVariantDto
{
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class TopCategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TotalProductsSold { get; set; }
    public decimal Revenue { get; set; }
}

public class PeriodOverviewDto
{
    public string PeriodLabel { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalProductsSold { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCancelledOrders { get; set; }
    public decimal Revenue { get; set; }
}

public class ReportResponseDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string GroupBy { get; set; } = string.Empty;

    public PeriodOverviewDto OverallOverview { get; set; } = new();
    public List<PeriodOverviewDto> PeriodBreakdowns { get; set; } = new();
    public List<TopProductVariantDto> TopProductVariants { get; set; } = new();
    public List<TopCategoryDto> TopCategories { get; set; } = new();
}
