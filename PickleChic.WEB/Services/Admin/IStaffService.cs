using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IStaffService
    {
        Task<ApiResult<List<StaffResponse>>> GetAllAsync(string? keyword = null);

        Task<ApiResult<StaffResponse>> GetByIdAsync(int id);

        Task<ApiResult<StaffResponse>> CreateAsync(StaffModel model);

        Task<ApiResult<StaffResponse>> UpdateAsync(StaffModel model);
    }
}