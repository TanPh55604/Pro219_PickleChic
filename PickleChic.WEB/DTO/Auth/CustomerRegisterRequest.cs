using System.Text.Json.Serialization;

namespace PickleChic.WEB.DTO.Auth
{
    public class CustomerRegisterRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public bool? Gender { get; set; }
    }
}