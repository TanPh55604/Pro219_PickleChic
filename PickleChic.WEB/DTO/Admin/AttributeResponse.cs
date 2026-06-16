namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeResponse
    {
        public int Id { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public List<AttributeValueResponse>? AttributeValues { get; set; }
    }
}
