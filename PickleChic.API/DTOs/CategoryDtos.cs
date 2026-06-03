namespace PickleChic.API.DTOs;

public class CategoryCreateDto
{
    public string Name { get; set; } = null!;
    public string? LinkImage { get; set; }
    public string? Description { get; set; }
    public int Status { get; set; }
}

public class CategoryUpdateDto : CategoryCreateDto
{
    public int Id { get; set; }
}
