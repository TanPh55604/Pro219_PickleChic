namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeValueResponse
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }

        public string? AttributeName { get; set; }
    }
}
