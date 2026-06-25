namespace PickleChic.API.DTOs;

public class WardCreateDto
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int DistrictId { get; set; }
}

public class WardUpdateDto : WardCreateDto
{
    public int Id { get; set; }
}
