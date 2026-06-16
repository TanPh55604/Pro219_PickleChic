using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IProductVariantService
    {
        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}
