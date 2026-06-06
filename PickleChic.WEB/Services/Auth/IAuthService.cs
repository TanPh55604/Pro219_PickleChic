using PickleChic.WEB.DTO.Auth;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResult<LoginResponse>> LoginCustomerAsync(LoginModel model);

        Task<ApiResult<LoginResponse>> LoginAdminAsync(LoginModel model);

        Task<ApiResult<bool>> RegisterCustomerAsync(RegisterModel model);

        Task<ApiResult<bool>> ChangeCustomerPasswordAsync(ChangePasswordModel model);

        Task<ApiResult<bool>> ForgotCustomerPasswordAsync(ForgotPasswordModel model);

        Task<ApiResult<bool>> ChangeAdminPasswordAsync(ChangePasswordModel model);

        Task LogoutAsync();
    }
}