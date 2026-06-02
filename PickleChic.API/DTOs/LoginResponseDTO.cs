namespace PickleChic.API.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool LoginSuccess { get; set; }
        public bool? FirstLogin { get; set; } = false;
    }
}
