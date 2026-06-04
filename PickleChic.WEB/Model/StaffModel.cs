namespace PickleChic.WEB.Model
{
    public class StaffModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string PasswordHash { get; set; } = "Admin12345@";

        public int RoleId { get; set; } = 1;

        public int Status { get; set; } = 1;

        public DateTime? LastLogin { get; set; }

        public bool IsActive => Status == 1;

        public bool IsFirstLogin => LastLogin is null;
    }
}
