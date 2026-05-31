using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class AttributeValue
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(ProductAttribute))]
    public int AttributeId { get; set; }

    public string Value { get; set; } = null!;

    public string? Note { get; set; }

    public ProductAttribute? ProductAttribute { get; set; }

    public ICollection<ProductVariantAttribute>? ProductVariantAttributes { get; set; }
}
