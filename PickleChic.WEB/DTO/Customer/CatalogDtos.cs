namespace PickleChic.WEB.DTO.Customer
{
    public class CustomerCategoryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? LinkImage { get; set; }

        public string? Description { get; set; }

        public int Status { get; set; }
    }

    public class CustomerBrandResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Status { get; set; }
    }
}
