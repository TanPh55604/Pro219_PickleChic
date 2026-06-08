namespace PickleChic.WEB.DTO.Admin
{
    public class CategoryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? LinkImage { get; set; }

        public string? Description { get; set; }

        public int Status { get; set; }

        public DateTime? InsertedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool Delete { get; set; }

        public bool IsActive => Status == 1;
    }
}