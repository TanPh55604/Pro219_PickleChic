using System.Text.Json.Serialization;

namespace PickleChic.WEB.DTO.Auth
{
    public class ResetPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}