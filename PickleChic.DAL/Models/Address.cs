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

    public string WardCode { get; set; } = null!;

    public string DistrictCode { get; set; } = null!;

    public string ProvinceCode { get; set; } = null!;

    public string DetailInfo { get; set; } = null!;

    public string WardName { get; set; } = null!;

    public string DistrictName { get; set; } = null!;

    public string ProvinceName { get; set; } = null!;

    public bool IsDefault { get; set; }

    public int Status { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime InsertedAt { get; set; }

    public bool Delete { get; set; }

    public Customer? Customer { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
