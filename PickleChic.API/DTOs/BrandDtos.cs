namespace PickleChic.API.DTOs;

public class BrandCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? UpdateBy { get; set; }
    public int Status { get; set; }
}

public class BrandUpdateDto : BrandCreateDto
{
    public int Id { get; set; }
}
