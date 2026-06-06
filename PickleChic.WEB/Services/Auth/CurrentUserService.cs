using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Auth;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public CurrentUserService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public async Task<ApiResult<AuthModel>> CheckCurrentUserAsync()
        {
            var result = await _apiProvider.GetAsync<CurrentUserResponse>(
                EndPointConfig.Auth.Check,
                requireAuth: true);

            if (!result.Success || result.Data is null)
            {
                return ApiResult<AuthModel>.Fail(
                    message: result.Message,
                    statusCode: result.StatusCode);
            }

            if (result.Data.IsExpired)
            {
                return ApiResult<AuthModel>.Fail(
                    message: "TokenExpired",
                    statusCode: result.StatusCode);
            }

            var mustChangePassword = await _authStorageService.MustChangePasswordAsync();

            var user = new AuthModel
            {
                Id = result.Data.Id,
                Username = result.Data.Username ?? string.Empty,
                FullName = result.Data.FullName ?? result.Data.Username ?? string.Empty,
                Email = result.Data.Email ?? string.Empty,
                Role = result.Data.Role ?? string.Empty,
                PhoneNumber = result.Data.PhoneNumber ?? string.Empty,
                RankId = result.Data.RankId ?? string.Empty,
                RankName = result.Data.RankName ?? string.Empty,
                TotalPoints = result.Data.TotalPoints,
                MustChangePassword = mustChangePassword
            };

            await _authStorageService.SaveUserAsync(user);

            return ApiResult<AuthModel>.Ok(
                data: user,
                message: "Success",
                statusCode: result.StatusCode);
        }
    }
}