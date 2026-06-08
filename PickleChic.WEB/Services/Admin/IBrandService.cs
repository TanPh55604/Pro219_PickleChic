using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IBrandService
    {
        Task<ApiResult<List<BrandResponse>>> GetAllAsync(string? keyword = null);

        Task<ApiResult<BrandResponse>> GetByIdAsync(int id);

        Task<ApiResult<BrandResponse>> CreateAsync(BrandModel model);

        Task<ApiResult<BrandResponse>> UpdateAsync(BrandModel model);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}