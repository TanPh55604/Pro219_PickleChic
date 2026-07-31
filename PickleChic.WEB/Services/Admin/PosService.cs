using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class PosService : IPosService
    {
        private readonly IApiProvider _apiProvider;

        public PosService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<ProductVariantSearchPageResponse>> SearchProductsAsync(
            string? keyword = null,
            int? brandId = null,
            int? categoryId = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            var url = EndPointConfig.Pos.Products(keyword, brandId, categoryId, pageNumber, pageSize);
            return await _apiProvider.GetAsync<ProductVariantSearchPageResponse>(url, requireAuth: true);
        }

        public async Task<ApiResult<PosStockCheckResponse>> CheckStockAsync(
            int productVariantId,
            int quantity)
        {
            return await _apiProvider.GetAsync<PosStockCheckResponse>(
                EndPointConfig.Pos.CheckStock(productVariantId, quantity),
                requireAuth: true);
        }

        public async Task<ApiResult<CustomerSearchPageResponse>> SearchCustomersAsync(
            string? keyword = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            return await _apiProvider.GetAsync<CustomerSearchPageResponse>(
                EndPointConfig.Pos.Customers(keyword, pageNumber, pageSize),
                requireAuth: true);
        }

        public async Task<ApiResult<List<VoucherResponse>>> GetVouchersByCustomerAsync(int customerId)
        {
            return await _apiProvider.GetAsync<List<VoucherResponse>>(
                EndPointConfig.Pos.Vouchers(customerId),
                requireAuth: true);
        }

        public async Task<ApiResult<OrderCalculationResult>> CalculateTotalAsync(
            PosOrderCalculationRequest request)
        {
            return await _apiProvider.PostAsync<PosOrderCalculationRequest, OrderCalculationResult>(
                EndPointConfig.Order.CalculateTotalPOS,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<UserOrderDetailResponse>> CheckoutAsync(PosCheckoutRequest request)
        {
            return await _apiProvider.PostAsync<PosCheckoutRequest, UserOrderDetailResponse>(
                EndPointConfig.Order.PosCheckout,
                request,
                requireAuth: true);
        }
    }
}
