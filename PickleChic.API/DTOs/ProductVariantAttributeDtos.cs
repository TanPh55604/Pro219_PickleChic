namespace PickleChic.API.DTOs;

public class ProductVariantAttributeCreateDto
{
    public int ProductVariantId { get; set; }
    public int AttributeValueId { get; set; }
}

public class ProductVariantAttributeUpdateDto : ProductVariantAttributeCreateDto
{
    public int Id { get; set; }
}
