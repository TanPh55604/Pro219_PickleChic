using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class ProductVariant
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    public string SKU { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int Status { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    public Product? Product { get; set; }

    public ICollection<ProductVariantImage>? ProductVariantImages { get; set; }

    public ICollection<ProductVariantAttribute>? ProductVariantAttributes { get; set; }

    public ICollection<CartItem>? CartItems { get; set; }

    public ICollection<OrderItem>? OrderItems { get; set; }

    public ICollection<PromotionDetail>? PromotionDetails { get; set; }
}
