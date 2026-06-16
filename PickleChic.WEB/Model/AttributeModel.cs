namespace PickleChic.WEB.Model
{
    public class AttributeModel
    {
        public int Id { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public List<AttributeValueModel> AttributeValues { get; set; } = new();
    }
}
