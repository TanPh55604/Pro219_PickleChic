namespace PickleChic.WEB.DTO.Admin
{
    public class BrandCreateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? UpdateBy { get; set; }

        public int Status { get; set; } = 1;
    }
}