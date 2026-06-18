namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeCreateRequest
    {
        public string AttributeName { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        public List<AttributeValueItemRequest> AttributeValues { get; set; } = new();
    }

    public class AttributeValueItemRequest
    {
        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
