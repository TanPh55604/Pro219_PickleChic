using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

[Table("ProductVariantAttributes")]
public class ProductVariantAttribute
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(ProductVariant))]
    public int ProductVariantId { get; set; }

    [ForeignKey(nameof(AttributeValue))]
    public int AttributeValueId { get; set; }

    public ProductVariant? ProductVariant { get; set; }

    public AttributeValue? AttributeValue { get; set; }
}
