namespace PickleChic.WEB.DTO.Admin
{
    public class ProductCreateRequest
    {
        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public int BrandId { get; set; }

        public int Status { get; set; } = 1;

        public string? UpdatedBy { get; set; }
    }
}
