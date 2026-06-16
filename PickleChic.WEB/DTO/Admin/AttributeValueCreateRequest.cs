namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeValueCreateRequest
    {
        public int AttributeId { get; set; }

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
