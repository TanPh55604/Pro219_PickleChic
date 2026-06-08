namespace PickleChic.WEB.DTO.Admin
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? LinkImage { get; set; }

        public string? Description { get; set; }

        public int Status { get; set; } = 1;
    }
}