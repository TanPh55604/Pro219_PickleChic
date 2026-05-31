using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }

    [ForeignKey(nameof(ProductVariant))]
    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public DateTime InsertedAt { get; set; }

    public Customer? Customer { get; set; }

    public ProductVariant? ProductVariant { get; set; }
}
