using System.Text.Json.Serialization;

namespace PickleChic.WEB.DTO.Auth
{
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; }

        [JsonPropertyName("loginSuccess")]
        public bool LoginSuccess { get; set; }

        [JsonPropertyName("firstLogin")]
        public bool FirstLogin { get; set; }
    }
}