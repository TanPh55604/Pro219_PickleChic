namespace PickleChic.API.DTOs;

public class StaffCreateDto
{
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; }
    public int Status { get; set; }
}

public class StaffUpdateDto : StaffCreateDto
{
    public int Id { get; set; }
    public DateTime? LastLogin { get; set; }
}
