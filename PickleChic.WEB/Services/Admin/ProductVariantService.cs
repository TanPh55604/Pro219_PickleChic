using PickleChic.WEB.Constant;
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

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.ProductVariant.Delete(id),
                requireAuth: true);
        }
    }
}
