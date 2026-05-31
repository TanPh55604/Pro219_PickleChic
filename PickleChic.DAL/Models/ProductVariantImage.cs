using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class ProductVariantImage
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(ProductVariant))]
    public int ProductVariantId { get; set; }

    public string URL { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsMain { get; set; }

    public ProductVariant? ProductVariant { get; set; }
}
