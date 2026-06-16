namespace PickleChic.WEB.Model
{
    public class ProductModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public int BrandId { get; set; }

        public int Status { get; set; }

        public bool IsActive => Status == 1;
    }
}
