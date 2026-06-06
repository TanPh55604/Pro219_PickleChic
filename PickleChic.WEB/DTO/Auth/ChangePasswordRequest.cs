using System.Text.Json.Serialization;

namespace PickleChic.WEB.DTO.Auth
{
    public class ChangePasswordRequest
    {
        [JsonPropertyName("currentPassword")]
        public string CurrentPassword { get; set; } = string.Empty;

        [JsonPropertyName("newHashPassword")]
        public string NewHashPassword { get; set; } = string.Empty;
    }
}