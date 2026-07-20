using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface IProductVariantService
    {
        Task<ApiResult<List<ProductVariantSearchResponse>>> GetByCategoryIdDetail(
            int categoryId,
            string? sortBy = null);

        Task<ApiResult<List<ProductVariantSearchResponse>>> GetByBrandIdDetail(
            int brandId,
            string? sortBy = null);
    }
}
