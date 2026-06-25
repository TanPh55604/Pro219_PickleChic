using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IVoucherService
    {
        Task<ApiResult<List<VoucherResponse>>> GetAllAsync(string? keyword = null);

        Task<ApiResult<VoucherResponse>> GetByIdAsync(int id);

        Task<ApiResult<VoucherResponse>> CreateAsync(VoucherModel model);

        Task<ApiResult<VoucherResponse>> UpdateAsync(VoucherModel model);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}
