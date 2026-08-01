using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly IApiProvider _apiProvider;

        public CustomerOrderService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<OrderCalculationResult>> CalculateTotalAsync(OrderCalculationRequest request)
        {
            return await _apiProvider.PostAsync<OrderCalculationRequest, OrderCalculationResult>(
                EndPointConfig.Order.CalculateTotal,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<CheckoutResponse>> CheckoutAsync(
            CheckoutRequest request,
            decimal discountAmount,
            decimal shippingFee,
            int paymentMethodTypeId,
            int addressId,
            int? voucherId = null,
            string? note = null,
            bool usePoints = false,
            bool bopis = false)
        {
            var url = EndPointConfig.Order.Checkout(
                discountAmount,
                shippingFee,
                paymentMethodTypeId,
                addressId,
                voucherId,
                note,
                usePoints,
                bopis);

            return await _apiProvider.PostAsync<CheckoutRequest, CheckoutResponse>(
                url,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<PaymentCallbackOrderResponse>> ConfirmPaymentSuccessAsync(int orderId, bool pos = false)
        {
            return await _apiProvider.GetAsync<PaymentCallbackOrderResponse>(
                EndPointConfig.Order.PaymentSuccess(orderId, pos),
                requireAuth: false);
        }

        public async Task<ApiResult<PaymentCallbackOrderResponse>> ConfirmPaymentCanceledAsync(int orderId)
        {
            return await _apiProvider.GetAsync<PaymentCallbackOrderResponse>(
                EndPointConfig.Order.PaymentCanceled(orderId),
                requireAuth: false);
        }

        public async Task<ApiResult<UserOrderDetailResponse>> GetUserOrderDetailAsync(int orderId)
        {
            return await _apiProvider.GetAsync<UserOrderDetailResponse>(
                EndPointConfig.Order.UserDetail(orderId),
                requireAuth: true);
        }

        public async Task<ApiResult<UserOrderDetailResponse>> CancelUserOrderAsync(int orderId)
        {
            return await _apiProvider.PostAsync<object, UserOrderDetailResponse>(
                EndPointConfig.Order.UserCancel(orderId),
                new { },
                requireAuth: true);
        }

        public async Task<ApiResult<List<UserOrderDetailResponse>>> GetUserOrdersAsync()
        {
            return await _apiProvider.GetAsync<List<UserOrderDetailResponse>>(
                EndPointConfig.Order.UserList,
                requireAuth: true);
        }

        public async Task<ApiResult<List<UserOrderDetailResponse>>> LookupOrdersAsync(
            string? orderCode = null,
            string? name = null,
            string? phoneNumber = null,
            bool requireAuth = false)
        {
            return await _apiProvider.GetAsync<List<UserOrderDetailResponse>>(
                EndPointConfig.Order.Lookup(orderCode, name, phoneNumber),
                requireAuth: requireAuth);
        }
    }
}
