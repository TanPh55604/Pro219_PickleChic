using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class Role
{
    [Key]
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Permissions { get; set; }

    public int Status { get; set; }

    public bool IsEdit { get; set; } = true;

    public ICollection<Staff>? StaffMembers { get; set; }
}
