namespace PickleChic.WEB.DTO.Admin
{
    public class StaffCreateRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string PasswordHash { get; set; } = "Admin12345@";

        public int RoleId { get; set; }

        public int Status { get; set; }
    }
}