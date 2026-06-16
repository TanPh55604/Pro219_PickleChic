namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeValueUpdateRequest
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
