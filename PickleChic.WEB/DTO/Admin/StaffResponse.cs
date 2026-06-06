namespace PickleChic.WEB.DTO.Admin
{
    public class StaffResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public int RoleId { get; set; }

        public int Status { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsActive => Status == 1;

        //public bool IsFirstLogin => LastLogin is null;
    }
}