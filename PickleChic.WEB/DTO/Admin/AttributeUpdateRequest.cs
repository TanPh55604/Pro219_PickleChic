namespace PickleChic.WEB.DTO.Admin
{
    public class AttributeUpdateRequest
    {
        public int Id { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
    }
}
