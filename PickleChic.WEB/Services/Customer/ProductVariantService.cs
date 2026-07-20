using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IApiProvider _apiProvider;

        public ProductVariantService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<ProductVariantSearchResponse>>> GetByCategoryIdDetail(
            int categoryId,
            string? sortBy = null)
        {
            return await _apiProvider.GetAsync<List<ProductVariantSearchResponse>>(
                EndPointConfig.ProductVariant.GetByCategory(categoryId, sortBy),
                requireAuth: false);
        }

        public async Task<ApiResult<List<ProductVariantSearchResponse>>> GetByBrandIdDetail(
            int brandId,
            string? sortBy = null)
        {
            return await _apiProvider.GetAsync<List<ProductVariantSearchResponse>>(
                EndPointConfig.ProductVariant.GetByBrand(brandId, sortBy),
                requireAuth: false);
        }
    }
}
