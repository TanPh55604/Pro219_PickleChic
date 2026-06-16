using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IProductService
    {
        Task<ApiResult<List<ProductDetailResponse>>> GetAllWithDetailsAsync(string? keyword = null);

        Task<ApiResult<ProductDetailResponse>> GetByIdWithDetailsAsync(int id);

        Task<ApiResult<ProductResponse>> CreateAsync(ProductModel model);

        Task<ApiResult<ProductResponse>> UpdateAsync(ProductModel model);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}
