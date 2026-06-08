using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiProvider _apiProvider;

        public CategoryService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<CategoryResponse>>> GetAllAsync(string? keyword = null)
        {
            var url = EndPointConfig.Category.GetAll;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword.Trim())}";
            }

            return await _apiProvider.GetAsync<List<CategoryResponse>>(
                url,
                requireAuth: true);
        }

        public async Task<ApiResult<CategoryResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<CategoryResponse>(
                EndPointConfig.Category.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<CategoryResponse>> CreateAsync(CategoryModel model)
        {
            var request = new CategoryCreateRequest
            {
                Name = model.Name,
                LinkImage = model.LinkImage,
                Description = model.Description,
                Status = model.Status
            };

            return await _apiProvider.PostAsync<CategoryCreateRequest, CategoryResponse>(
                EndPointConfig.Category.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<CategoryResponse>> UpdateAsync(CategoryModel model)
        {
            var request = new CategoryUpdateRequest
            {
                Id = model.Id,
                Name = model.Name,
                LinkImage = model.LinkImage,
                Description = model.Description,
                Status = model.Status
            };

            return await _apiProvider.PatchAsync<CategoryUpdateRequest, CategoryResponse>(
                EndPointConfig.Category.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Category.Delete(id),
                requireAuth: true);
        }
    }
}