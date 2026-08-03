namespace PickleChic.API.DTOs;

public class RoleCreateDto
{
    public string RoleName { get; set; } = null!;
    public string? Permissions { get; set; }
    public int Status { get; set; }
}

public class RoleUpdateDto : RoleCreateDto
{
    public int Id { get; set; }
}
