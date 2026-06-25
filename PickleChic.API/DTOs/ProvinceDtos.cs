namespace PickleChic.API.DTOs;

public class ProvinceCreateDto
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}

public class ProvinceUpdateDto : ProvinceCreateDto
{
    public int Id { get; set; }
}

public class ProvinceResultDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}
