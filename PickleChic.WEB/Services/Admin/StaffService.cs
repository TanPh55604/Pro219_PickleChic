using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class StaffService : IStaffService
    {
        private readonly IApiProvider _apiProvider;

        public StaffService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<StaffResponse>>> GetAllAsync(string? keyword = null)
        {
            var url = EndPointConfig.Staff.GetAll;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword)}";
            }

            return await _apiProvider.GetAsync<List<StaffResponse>>(
                url,
                requireAuth: true);
        }

        public async Task<ApiResult<StaffResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<StaffResponse>(
                EndPointConfig.Staff.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<StaffResponse>> CreateAsync(StaffModel model)
        {
            var request = new StaffCreateRequest
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = "Admin12345@",
                RoleId = model.RoleId,
                Status = model.Status
            };

            return await _apiProvider.PostAsync<StaffCreateRequest, StaffResponse>(
                EndPointConfig.Staff.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<StaffResponse>> UpdateAsync(StaffModel model)
        {
            var request = new StaffUpdateRequest
            {
                Id = model.Id,
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = model.PasswordHash,
                RoleId = model.RoleId,
                Status = model.Status,
                //LastLogin = model.LastLogin
            };

            return await _apiProvider.PatchAsync<StaffUpdateRequest, StaffResponse>(
                EndPointConfig.Staff.Update,
                request,
                requireAuth: true);
        }
    }
}