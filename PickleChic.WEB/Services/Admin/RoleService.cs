using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IRoleService
    {
        Task<ApiResult<List<RoleResponse>>> GetAllAsync(string? keyword = null);

        Task<ApiResult<RoleResponse>> GetByIdAsync(int id);

        Task<ApiResult<RoleResponse>> CreateAsync(RoleCreateRequest request);

        Task<ApiResult<RoleResponse>> UpdateAsync(RoleUpdateRequest request);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }

    public class RoleService : IRoleService
    {
        private readonly IApiProvider _apiProvider;

        public RoleService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<RoleResponse>>> GetAllAsync(string? keyword = null)
        {
            var url = EndPointConfig.Role.GetAll;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword.Trim())}";
            }

            return await _apiProvider.GetAsync<List<RoleResponse>>(url, requireAuth: true);
        }

        public async Task<ApiResult<RoleResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<RoleResponse>(
                EndPointConfig.Role.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<RoleResponse>> CreateAsync(RoleCreateRequest request)
        {
            request.RoleName = request.RoleName.Trim();

            return await _apiProvider.PostAsync<RoleCreateRequest, RoleResponse>(
                EndPointConfig.Role.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<RoleResponse>> UpdateAsync(RoleUpdateRequest request)
        {
            request.RoleName = request.RoleName.Trim();

            return await _apiProvider.PatchAsync<RoleUpdateRequest, RoleResponse>(
                EndPointConfig.Role.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Role.Delete(id),
                requireAuth: true);
        }
    }
}
