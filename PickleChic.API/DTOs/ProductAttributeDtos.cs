namespace PickleChic.API.DTOs;

public class ProductAttributeCreateDto
{
    public string AttributeName { get; set; } = null!;
}

public class ProductAttributeUpdateDto : ProductAttributeCreateDto
{
    public int Id { get; set; }
}

public class ProductAttributeDto
{
    public int Id { get; set; }
    public string AttributeName { get; set; } = null!;
    public List<AttributeValueDto>? AttributeValues { get; set; }
}

public class AttributeValueCreateWithoutIdDto
{
    public string Value { get; set; } = null!;
    public string? Note { get; set; }
}

public class ProductAttributeWithValuesCreateDto
{
    public string AttributeName { get; set; } = null!;
    public List<AttributeValueCreateWithoutIdDto> AttributeValues { get; set; } = new();
}

