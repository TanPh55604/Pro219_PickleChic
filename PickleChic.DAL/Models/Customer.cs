using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Customer
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public bool? Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public int TotalPoints { get; set; }

    public DateTime? LastLogin { get; set; }

    public int Status { get; set; }

    [ForeignKey(nameof(Rank))]
    public int RankId { get; set; }

    public Rank? Rank { get; set; }

    public ICollection<Address>? Addresses { get; set; }

    public ICollection<CartItem>? CartItems { get; set; }

    public ICollection<Wishlist>? Wishlists { get; set; }

    public ICollection<PointHistory>? PointHistories { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
