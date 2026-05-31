using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class PaymentMethod
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime InsertedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Delete { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
