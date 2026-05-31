using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class Brand
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? UpdateBy { get; set; }

    public int Status { get; set; }

    public bool Delete { get; set; }

    public DateTime InsertedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Product>? Products { get; set; }
}
