using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Storage;

namespace PickleChic.WEB.Services.Auth
{
    public class AuthStorageService : IAuthStorageService
    {
        private const string AccessTokenKey = "picklechic_access_token";
        private const string RefreshTokenKey = "picklechic_refresh_token";
        private const string UserKey = "picklechic_user";
        private const string MustChangePasswordKey = "picklechic_must_change_password";

        private readonly ILocalStorageService _localStorage;

        public AuthStorageService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveLoginAsync(
            string accessToken,
            string? refreshToken,
            bool mustChangePassword)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentException("Access token is required", nameof(accessToken));
            }

            await _localStorage.SetItemAsync(AccessTokenKey, accessToken);
            await _localStorage.SetItemAsync(MustChangePasswordKey, mustChangePassword);

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
            }
            else
            {
                await _localStorage.RemoveItemAsync(RefreshTokenKey);
            }
        }

        public async Task SetMustChangePasswordAsync(bool value)
        {
            await _localStorage.SetItemAsync(MustChangePasswordKey, value);
        }

        public async Task SaveUserAsync(AuthModel user)
        {
            await _localStorage.SetItemAsync(UserKey, user);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(AccessTokenKey);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(RefreshTokenKey);
        }

        public async Task<AuthModel?> GetUserAsync()
        {
            return await _localStorage.GetItemAsync<AuthModel>(UserKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetAccessTokenAsync();

            return !string.IsNullOrWhiteSpace(token);
        }

        public async Task<bool> MustChangePasswordAsync()
        {
            return await _localStorage.GetItemAsync<bool>(MustChangePasswordKey);
        }

        public async Task ClearAuthAsync()
        {
            await _localStorage.RemoveItemAsync(AccessTokenKey);
            await _localStorage.RemoveItemAsync(RefreshTokenKey);
            await _localStorage.RemoveItemAsync(UserKey);
            await _localStorage.RemoveItemAsync(MustChangePasswordKey);
        }
    }
}