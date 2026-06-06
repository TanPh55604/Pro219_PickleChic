using PickleChic.WEB.Model;

namespace PickleChic.WEB.Services.Auth
{
    public interface IAuthStorageService
    {
        Task SaveLoginAsync(
            string accessToken,
            string? refreshToken,
            bool mustChangePassword);

        Task SaveUserAsync(AuthModel user);

        Task<string?> GetAccessTokenAsync();

        Task<string?> GetRefreshTokenAsync();

        Task<AuthModel?> GetUserAsync();

        Task<bool> IsAuthenticatedAsync();

        Task<bool> MustChangePasswordAsync();

        Task SetMustChangePasswordAsync(bool value);

        Task ClearAuthAsync();
    }
}