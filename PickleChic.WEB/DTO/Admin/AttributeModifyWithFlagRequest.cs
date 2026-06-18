namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeModifyWithFlagRequest
    {
        public int Id { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        public List<AttributeValueModifyWithFlagRequest> AttributeValues { get; set; } = new();
    }

    public class AttributeValueModifyWithFlagRequest
    {
        public int Id { get; set; }

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }

        public int FlagAction { get; set; }
    }
}
