using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface IProductService
    {
        Task<ApiResult<HomeProductsResponse>> GetHomeProductsAsync();

        Task<ApiResult<ProductFilterPageResponse>> FilterAsync(ProductSearchQuery query);

        Task<ApiResult<ProductDetailResponse>> GetByIdWithDetailsAsync(int id);

        Task<ApiResult<List<ProductVariantSearchResponse>>> GetRelatedByCategoryAsync(
            int categoryId,
            int excludeProductId,
            int limit = 6);
    }
}
