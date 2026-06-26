using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface IProductService
    {
        Task<ApiResult<ProductVariantSearchPageResponse>> SearchAsync(ProductSearchQuery query);
    }
}
