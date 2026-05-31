using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class Role
{
    [Key]
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Permissions { get; set; }

    public int Status { get; set; }

    public ICollection<Staff>? StaffMembers { get; set; }
}
