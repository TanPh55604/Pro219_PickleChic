using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class PagePermission
{
    [Key]
    public int Id { get; set; }

    public string? PageCode { get; set; }

    public string? PageRoute { get; set; }

    public string? AvailablePermissions { get; set; }

    public string? DefaultPermissions { get; set; }
}
