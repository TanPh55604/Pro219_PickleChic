using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class District
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = null!;

    [ForeignKey(nameof(Province))]
    public int ProvinceId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime InsertedAt { get; set; }

    public Province? Province { get; set; }

    public ICollection<Ward>? Wards { get; set; }
}
