using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Address
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string DetailInfo { get; set; } = null!;

    [ForeignKey(nameof(Ward))]
    [Column("WardID")]
    public int WardId { get; set; }

    public bool IsDefault { get; set; }

    public int Status { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime InsertedAt { get; set; }

    public bool Delete { get; set; }

    public Customer? Customer { get; set; }

    public Ward? Ward { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
