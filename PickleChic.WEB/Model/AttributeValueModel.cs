namespace PickleChic.WEB.Model
{
    public class AttributeValueModel
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }

        public bool IsNew { get; set; }

        public bool IsDeleted { get; set; }
    }
}
