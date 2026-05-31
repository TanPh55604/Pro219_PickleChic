using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }

    [ForeignKey(nameof(ProductVariant))]
    public int ProductVariantId { get; set; }

    [ForeignKey(nameof(Promotion))]
    public int? PromotionId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    public bool IsReviewed { get; set; }

    public DateTime InsertedAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? UpdateBy { get; set; }

    public bool Delete { get; set; }

    public DateTime? DeleteAt { get; set; }

    public Order? Order { get; set; }

    public ProductVariant? ProductVariant { get; set; }

    public Promotion? Promotion { get; set; }
}
