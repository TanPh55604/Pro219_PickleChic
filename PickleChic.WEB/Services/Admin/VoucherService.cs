using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class VoucherService : IVoucherService
    {
        private readonly IApiProvider _apiProvider;

        public VoucherService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<VoucherResponse>>> GetAllAsync(string? keyword = null)
        {
            var url = EndPointConfig.Voucher.Management.GetAll;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword.Trim())}";
            }

            return await _apiProvider.GetAsync<List<VoucherResponse>>(
                url,
                requireAuth: true);
        }

        public async Task<ApiResult<VoucherResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<VoucherResponse>(
                EndPointConfig.Voucher.Management.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<VoucherResponse>> CreateAsync(VoucherModel model)
        {
            var request = MapToCreateRequest(model);
            request.UsedCount = 0;

            return await _apiProvider.PostAsync<VoucherCreateRequest, VoucherResponse>(
                EndPointConfig.Voucher.Management.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<VoucherResponse>> UpdateAsync(VoucherModel model)
        {
            var request = MapToUpdateRequest(model);

            return await _apiProvider.PatchAsync<VoucherUpdateRequest, VoucherResponse>(
                EndPointConfig.Voucher.Management.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Voucher.Management.Delete(id),
                requireAuth: true);
        }

        private static VoucherCreateRequest MapToCreateRequest(VoucherModel model)
        {
            return new VoucherCreateRequest
            {
                VoucherCode = model.VoucherCode.Trim(),
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                MinOrderValue = model.MinOrderValue,
                MaxDiscountAmount = model.DiscountType == VoucherDiscountType.Fixed
                    ? null
                    : model.MaxDiscountAmount,
                MinimumSpend = model.MinimumSpend,
                StartDate = model.StartDate!.Value,
                EndDate = model.EndDate!.Value,
                UsageLimit = model.UsageLimit,
                CustomerUsageLimit = model.CustomerUsageLimit,
                UsedCount = model.UsedCount,
                IsActive = IsWithinActivePeriod(model.StartDate!.Value, model.EndDate!.Value)
            };
        }

        private static VoucherUpdateRequest MapToUpdateRequest(VoucherModel model)
        {
            var request = MapToCreateRequest(model);

            return new VoucherUpdateRequest
            {
                Id = model.Id,
                VoucherCode = request.VoucherCode,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MinOrderValue = request.MinOrderValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinimumSpend = request.MinimumSpend,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UsageLimit = request.UsageLimit,
                CustomerUsageLimit = request.CustomerUsageLimit,
                UsedCount = model.UsedCount,
                IsActive = request.IsActive
            };
        }

        private static bool IsWithinActivePeriod(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;
            return startDate <= now && endDate > now;
        }
    }
}
