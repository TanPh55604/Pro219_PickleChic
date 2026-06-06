using System.Text.Json.Serialization;

namespace PickleChic.WEB.DTO.Auth
{
    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}