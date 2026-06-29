using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Voucher
{
    [Key]
    public int Id { get; set; }

    public string VoucherCode { get; set; } = null!;

    public string DiscountType { get; set; } = null!; //  "Percentage" or "FixedAmount"

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinOrderValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxDiscountAmount { get; set; }

    public int? MinimumPointRank { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int UsageLimit { get; set; }

    public int CustomerUsageLimit { get; set; }

    public int UsedCount { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
