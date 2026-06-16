using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;

namespace PickleChic.WEB.Services.Admin
{
    public class ProductService : IProductService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public ProductService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public async Task<ApiResult<List<ProductDetailResponse>>> GetAllWithDetailsAsync(string? keyword = null)
        {
            var url = EndPointConfig.Product.GetAllWithDetails;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword.Trim())}";
            }

            return await _apiProvider.GetAsync<List<ProductDetailResponse>>(
                url,
                requireAuth: true);
        }

        public async Task<ApiResult<ProductDetailResponse>> GetByIdWithDetailsAsync(int id)
        {
            return await _apiProvider.GetAsync<ProductDetailResponse>(
                EndPointConfig.Product.GetByIdWithDetails(id),
                requireAuth: true);
        }

        public async Task<ApiResult<ProductResponse>> CreateAsync(ProductModel model)
        {
            var request = new ProductCreateRequest
            {
                ProductName = model.ProductName.Trim(),
                Description = model.Description?.Trim(),
                CategoryId = model.CategoryId,
                BrandId = model.BrandId,
                Status = 0,
                UpdatedBy = await GetCurrentUserNameAsync()
            };

            return await _apiProvider.PostAsync<ProductCreateRequest, ProductResponse>(
                EndPointConfig.Product.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<ProductResponse>> UpdateAsync(ProductModel model)
        {
            var request = new ProductUpdateRequest
            {
                Id = model.Id,
                ProductName = model.ProductName.Trim(),
                Description = model.Description?.Trim(),
                CategoryId = model.CategoryId,
                BrandId = model.BrandId,
                Status = model.Status,
                UpdatedBy = await GetCurrentUserNameAsync()
            };

            return await _apiProvider.PatchAsync<ProductUpdateRequest, ProductResponse>(
                EndPointConfig.Product.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Product.Delete(id),
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
