using System.Diagnostics.Contracts;

namespace PickleChic.API.DTOs
{
    public class RegisterModel
    {
        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PasswordHash { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool? Gender { get; set; }
    }
}
