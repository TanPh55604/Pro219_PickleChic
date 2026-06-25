using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public class VoucherService : IVoucherService
    {
        private readonly IApiProvider _apiProvider;

        public VoucherService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<VoucherResponse>>> GetOngoingAsync()
        {
            var result = await _apiProvider.GetAsync<List<VoucherResponse>>(
                EndPointConfig.Voucher.GetAvailable,
                requireAuth: true);

            if (!result.Success || result.Data is null)
            {
                return result;
            }

            result.Data = result.Data
                .Where(IsOngoing)
                .OrderBy(v => v.EndDate)
                .ToList();

            return result;
        }

        public static bool IsOngoing(VoucherResponse voucher)
        {
            var now = DateTime.Now;

            return voucher.IsActive
                && voucher.StartDate <= now
                && voucher.EndDate >= now
                && voucher.UsedCount < voucher.UsageLimit;
        }
    }
}
