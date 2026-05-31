using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Staff
{
    [Key]
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    public DateTime? LastLogin { get; set; }

    public int Status { get; set; }

    public Role? Role { get; set; }
}
