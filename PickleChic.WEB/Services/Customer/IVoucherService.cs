using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface IVoucherService
    {
        Task<ApiResult<List<VoucherResponse>>> GetOngoingAsync();
    }
}
