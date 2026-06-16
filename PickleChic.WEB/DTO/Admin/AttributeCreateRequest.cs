namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeCreateRequest
    {
        public string AttributeName { get; set; } = string.Empty;

        public List<AttributeValueItemRequest> AttributeValues { get; set; } = new();
    }

    public class AttributeValueItemRequest
    {
        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
