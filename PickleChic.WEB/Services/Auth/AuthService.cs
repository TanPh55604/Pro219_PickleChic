using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Auth;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;
using System.Security.Cryptography;
using System.Text;

namespace PickleChic.WEB.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public AuthService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public async Task<ApiResult<LoginResponse>> LoginCustomerAsync(LoginModel model)
        {
            var hashPassword = HashPassword(model.Password);
            var request = new LoginRequest
            {
                Username = model.Email,
                PasswordHash = hashPassword,
            };

            var result = await _apiProvider.PostAsync<LoginRequest, LoginResponse>(
                EndPointConfig.Auth.LoginCustomer,
                request);

            if (!result.Success || result.Data is null)
            {
                return result;
            }

            if (!result.Data.LoginSuccess || string.IsNullOrWhiteSpace(result.Data.Token))
            {
                return ApiResult<LoginResponse>.Fail(
                    message: "Đăng nhập không thành công",
                    statusCode: result.StatusCode);
            }

            await _authStorageService.SaveLoginAsync(
                accessToken: result.Data.Token,
                refreshToken: null,
                mustChangePassword: result.Data.FirstLogin);

            return result;
        }

        public async Task<ApiResult<LoginResponse>> LoginAdminAsync(LoginModel model)
        {
            var hashPassword = HashPassword(model.Password);
            var request = new LoginRequest
            {
                Username = model.Email,
                PasswordHash = hashPassword,
            };

            var result = await _apiProvider.PostAsync<LoginRequest, LoginResponse>(
                EndPointConfig.Auth.LoginStaff,
                request);

            if (!result.Success || result.Data is null)
            {
                return result;
            }

            if (!result.Data.LoginSuccess || string.IsNullOrWhiteSpace(result.Data.Token))
            {
                return ApiResult<LoginResponse>.Fail(
                    message: "Đăng nhập không thành công",
                    statusCode: result.StatusCode);
            }

            await _authStorageService.SaveLoginAsync(
                accessToken: result.Data.Token,
                refreshToken: null,
                mustChangePassword: result.Data.FirstLogin || false);

            return result;
        }

        public async Task<ApiResult<bool>> RegisterCustomerAsync(RegisterModel model)
        {
            var hashPassword = HashPassword(model.Password);
            var request = new CustomerRegisterRequest
            {
                UserName = model.Email,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = hashPassword,
                DateOfBirth = model.DateOfBirth,
                Gender = ConvertGender(model.Gender)
            };

            return await _apiProvider.PostAsync<CustomerRegisterRequest, bool>(
                EndPointConfig.Auth.CustomerRegister,
                request);
        }

        public async Task<ApiResult<bool>> ChangeCustomerPasswordAsync(ChangePasswordModel model)
        {
            string currentHash = HashPassword(model.CurrentPassword);
            string newHash = HashPassword(model.NewPassword);

            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentHash,
                NewHashPassword = newHash
            };

            var result = await _apiProvider.PostAsync<ChangePasswordRequest, bool>(
                EndPointConfig.Auth.ChangePasswordCustomer,
                request,
                requireAuth: true);

            if (!result.Success)
            {
                return result;
            }

            await _authStorageService.SetMustChangePasswordAsync(false);

            return result;
        }

        public async Task<ApiResult<bool>> ForgotCustomerPasswordAsync(ForgotPasswordModel model)
        {
            var request = new ResetPasswordRequest
            {
                Email = model.Email
            };

            return await _apiProvider.PostAsync<ResetPasswordRequest, bool>(
                EndPointConfig.Auth.ResetPassword,
                request);
        }

        public async Task<ApiResult<bool>> ChangeAdminPasswordAsync(ChangePasswordModel model)
        {
            string currentHash = HashPassword(model.CurrentPassword);
            string newHash = HashPassword(model.NewPassword);

            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentHash,
                NewHashPassword = newHash
            };

            var result = await _apiProvider.PostAsync<ChangePasswordRequest, bool>(
                EndPointConfig.Auth.ChangePasswordStaff,
                request,
                requireAuth: true);

            if (!result.Success)
            {
                return result;
            }

            await _authStorageService.SetMustChangePasswordAsync(false);

            return result;
        }

        public async Task LogoutAsync()
        {
            await _authStorageService.ClearAuthAsync();
        }

        private static bool? ConvertGender(int gender)
        {
            if (gender == 2)
            {
                return null;
            }

            return gender switch
            {
                1 => true,
                0 => false,
                _ => null
            };
        }

        public string HashPassword(string password) 
        { 
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            md5.Clear();
            return sb.ToString();

        }
    }
}