using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

[Table("Attribute")]
public class ProductAttribute
{
    [Key]
    public int Id { get; set; }

    public string AttributeName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public ICollection<AttributeValue>? AttributeValues { get; set; }
}
