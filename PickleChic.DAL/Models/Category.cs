using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LinkImage { get; set; }

    public string? Description { get; set; }

    public int Status { get; set; }

    public DateTime InsertedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Delete { get; set; }

    public ICollection<Product>? Products { get; set; }
    public ICollection<ProductAttribute>? ProductAttributes { get; set; }
}
