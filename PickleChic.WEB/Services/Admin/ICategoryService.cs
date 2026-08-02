using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface ICategoryService
    {
        Task<ApiResult<List<CategoryResponse>>> GetAllAsync(string? keyword = null);

        Task<ApiResult<CategoryResponse>> GetByIdAsync(int id);

        Task<ApiResult<CategoryResponse>> CreateAsync(CategoryModel model);

        Task<ApiResult<CategoryResponse>> UpdateAsync(CategoryModel model);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}