namespace PickleChic.API.DTOs;

public class PagePermissionCreateDto
{
    public string? PageCode { get; set; }
    public string? PageRoute { get; set; }
    public string? AvailablePermissions { get; set; }
    public string? DefaultPermissions { get; set; }
}

public class PagePermissionUpdateDto : PagePermissionCreateDto
{
    public int Id { get; set; }
}

public class PagePermissionDTO
{
    public string PageCode { get; set; } = string.Empty;
    public string PagePermissions { get; set; } = string.Empty;
}
