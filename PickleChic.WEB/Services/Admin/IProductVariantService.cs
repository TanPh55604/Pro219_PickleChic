using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IProductVariantService
    {
        Task<ApiResult<ProductVariantSummaryResponse>> GetByIdWithDetailsAsync(int id);

        Task<ApiResult<ProductVariantMutationResponse>> CreateWithAttributesAsync(
            ProductVariantModel model,
            List<int> attributeValueIds);

        Task<ApiResult<ProductVariantMutationResponse>> UpdateWithAttributesAsync(
            ProductVariantModel model,
            List<int> attributeValueIds);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}
