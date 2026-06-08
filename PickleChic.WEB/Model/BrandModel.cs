namespace PickleChic.WEB.Model
{
    public class BrandModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Status { get; set; } = 1;

        public bool IsActive => Status == 1;
    }
}