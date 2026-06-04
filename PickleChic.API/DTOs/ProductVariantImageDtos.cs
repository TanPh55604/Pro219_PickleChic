namespace PickleChic.API.DTOs;

public class ProductVariantImageCreateDto
{
    public int ProductVariantId { get; set; }
    public string URL { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsMain { get; set; }
}

public class ProductVariantImageUpdateDto : ProductVariantImageCreateDto
{
    public int Id { get; set; }
}
