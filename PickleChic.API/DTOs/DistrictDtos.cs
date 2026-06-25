namespace PickleChic.API.DTOs;

public class DistrictCreateDto
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int ProvinceId { get; set; }
}

public class DistrictUpdateDto : DistrictCreateDto
{
    public int Id { get; set; }
}
