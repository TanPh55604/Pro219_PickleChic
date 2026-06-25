using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Ward
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = null!;

    [ForeignKey(nameof(District))]
    public int DistrictId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime InsertedAt { get; set; }

    public District? District { get; set; }

    public ICollection<Address>? Addresses { get; set; }
}
