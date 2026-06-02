namespace PickleChic.API.DTOs
{
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewHashPassword { get; set; } = string.Empty;
    }
}
