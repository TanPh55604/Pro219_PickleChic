namespace PickleChic.WEB.Model
{
    public class AuthModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string RankId { get; set; } = string.Empty;

        public string RankName { get; set; } = string.Empty;

        public int TotalPoints { get; set; }

        public bool MustChangePassword { get; set; }

        public bool IsCustomer => Role == "Customer";

        public bool IsAdmin => Role != "Customer";
    }
}