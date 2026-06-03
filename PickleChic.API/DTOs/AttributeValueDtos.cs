namespace PickleChic.API.DTOs;

public class AttributeValueCreateDto
{
    public int AttributeId { get; set; }
    public string Value { get; set; } = null!;
    public string? Note { get; set; }
}

public class AttributeValueUpdateDto : AttributeValueCreateDto
{
    public int Id { get; set; }
}

public class AttributeValueDto
{
    public int Id { get; set; }
    public int AttributeId { get; set; }
    public string Value { get; set; } = null!;
    public string? Note { get; set; }
    public string? AttributeName { get; set; }
}

