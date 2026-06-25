using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class Province
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public DateTime InsertedAt { get; set; }

    public ICollection<District>? Districts { get; set; }
}
