using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;

namespace PickleChic.WEB.Services.Admin
{
    public class BrandService : IBrandService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public BrandService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public async Task<ApiResult<List<BrandResponse>>> GetAllAsync(string? keyword = null)
        {
            var url = EndPointConfig.Brand.GetAll;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword.Trim())}";
            }

            return await _apiProvider.GetAsync<List<BrandResponse>>(
                url,
                requireAuth: true);
        }

        public async Task<ApiResult<BrandResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<BrandResponse>(
                EndPointConfig.Brand.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<BrandResponse>> CreateAsync(BrandModel model)
        {
            var updateBy = await GetCurrentUserNameAsync();

            var request = new BrandCreateRequest
            {
                Name = model.Name,
                Description = model.Description,
                UpdateBy = updateBy,
                Status = model.Status
            };

            return await _apiProvider.PostAsync<BrandCreateRequest, BrandResponse>(
                EndPointConfig.Brand.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<BrandResponse>> UpdateAsync(BrandModel model)
        {
            var updateBy = await GetCurrentUserNameAsync();

            var request = new BrandUpdateRequest
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                UpdateBy = updateBy,
                Status = model.Status
            };

            return await _apiProvider.PatchAsync<BrandUpdateRequest, BrandResponse>(
                EndPointConfig.Brand.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Brand.Delete(id),
                requireAuth: true);
        }

        private async Task<string> GetCurrentUserNameAsync()
        {
            var user = await _authStorageService.GetUserAsync();

            if (!string.IsNullOrWhiteSpace(user?.FullName))
            {
                return user.FullName;
            }

            if (!string.IsNullOrWhiteSpace(user?.Username))
            {
                return user.Username;
            }

            return "Admin";
        }
    }
}