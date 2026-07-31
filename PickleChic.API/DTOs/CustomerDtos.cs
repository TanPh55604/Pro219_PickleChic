namespace PickleChic.API.DTOs;

public class CustomerCreateDto
{
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public bool? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int TotalPoints { get; set; }
    public int Status { get; set; }
    public int RankId { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class CustomerUpdateDto : CustomerCreateDto
{
    public int Id { get; set; }
}

public class CustomerSearchResultDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public bool? Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int TotalPoints { get; set; }
    public int Status { get; set; }
    public int RankId { get; set; }
    public string? RankName { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class CustomerSearchPageDto
{
    public List<CustomerSearchResultDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
