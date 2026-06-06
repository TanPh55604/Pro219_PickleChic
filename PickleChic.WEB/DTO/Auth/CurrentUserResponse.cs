namespace PickleChic.WEB.DTO.Auth
{
    public class CurrentUserResponse
    {
        public int Id { get; set; }

        public string? Username { get; set; } = null;

        public string? Role { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? RankId { get; set; }

        public string? RankName { get; set; }

        public int TotalPoints { get; set; }

        public DateTime? ExpirationTime { get; set; }

        public bool IsExpired { get; set; }
    }
}