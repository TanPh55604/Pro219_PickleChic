namespace PickleChic.WEB.Model
{
    public class CustomerModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = "Customer12345@";

        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-DateTime.Today.Year + 20);

        public int TotalPoints { get; set; }

        public bool Status { get; set; } = true;

        public int RankId { get; set; } = 1;

        public string? RankName { get; set; }

        public DateTime? LastLogin { get; set; }

        public string GenderText => Gender switch
        {
            true => "Nam",
            false => "Nữ",
            null => "Chưa chọn"
        };

        public bool IsFirstLogin => LastLogin is null;
    }
}
