using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IApiProvider _apiProvider;

        public ProductVariantService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<ProductVariantSummaryResponse>> GetByIdWithDetailsAsync(int id)
        {
            return await _apiProvider.GetAsync<ProductVariantSummaryResponse>(
                EndPointConfig.ProductVariant.GetByIdWithDetails(id),
                requireAuth: true);
        }

        public async Task<ApiResult<ProductVariantMutationResponse>> CreateWithAttributesAsync(
            ProductVariantModel model,
            List<int> attributeValueIds)
        {
            var request = new ProductVariantCreateRequest
            {
                ProductId = model.ProductId,
                SKU = model.SKU.Trim(),
                VariantName = model.VariantName?.Trim(),
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Status = model.Status,
                AttributeValueIds = attributeValueIds
            };

            return await _apiProvider.PostAsync<ProductVariantCreateRequest, ProductVariantMutationResponse>(
                EndPointConfig.ProductVariant.CreateWithAttributes,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<ProductVariantMutationResponse>> UpdateWithAttributesAsync(
            ProductVariantModel model,
            List<int> attributeValueIds)
        {
            var request = new ProductVariantUpdateRequest
            {
                Id = model.Id,
                ProductId = model.ProductId,
                SKU = model.SKU.Trim(),
                VariantName = model.VariantName?.Trim(),
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Status = model.Status,
                AttributeValueIds = attributeValueIds
            };

            return await _apiProvider.PatchAsync<ProductVariantUpdateRequest, ProductVariantMutationResponse>(
                EndPointConfig.ProductVariant.UpdateWithAttributes,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.ProductVariant.Delete(id),
                requireAuth: true);
        }
    }
}
