namespace PickleChic.WEB.Model
{
    public class RegisterModel
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; } = string.Empty;
    }
}
