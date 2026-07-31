using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IPosService
    {
        Task<ApiResult<ProductVariantSearchPageResponse>> SearchProductsAsync(
            string? keyword = null,
            int? brandId = null,
            int? categoryId = null,
            int pageNumber = 1,
            int pageSize = 20);

        Task<ApiResult<PosStockCheckResponse>> CheckStockAsync(int productVariantId, int quantity);

        Task<ApiResult<CustomerSearchPageResponse>> SearchCustomersAsync(
            string? keyword = null,
            int pageNumber = 1,
            int pageSize = 20);

        Task<ApiResult<List<VoucherResponse>>> GetVouchersByCustomerAsync(int customerId);

        Task<ApiResult<OrderCalculationResult>> CalculateTotalAsync(PosOrderCalculationRequest request);

        Task<ApiResult<UserOrderDetailResponse>> CheckoutAsync(PosCheckoutRequest request);
    }
}
