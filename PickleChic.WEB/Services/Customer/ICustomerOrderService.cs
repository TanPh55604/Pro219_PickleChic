using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface ICustomerOrderService
    {
        Task<ApiResult<OrderCalculationResult>> CalculateTotalAsync(OrderCalculationRequest request);

        Task<ApiResult<CheckoutResponse>> CheckoutAsync(
            CheckoutRequest request,
            decimal discountAmount,
            decimal shippingFee,
            int paymentMethodTypeId,
            int addressId,
            int? voucherId = null,
            string? note = null,
            bool usePoints = false,
            bool bopis = false);

        Task<ApiResult<PaymentCallbackOrderResponse>> ConfirmPaymentSuccessAsync(int orderId, bool pos = false);

        Task<ApiResult<PaymentCallbackOrderResponse>> ConfirmPaymentCanceledAsync(int orderId);

        Task<ApiResult<UserOrderDetailResponse>> GetUserOrderDetailAsync(int orderId);

        Task<ApiResult<UserOrderDetailResponse>> CancelUserOrderAsync(int orderId);

        Task<ApiResult<List<UserOrderDetailResponse>>> GetUserOrdersAsync();

        Task<ApiResult<List<UserOrderDetailResponse>>> LookupOrdersAsync(
            string? orderCode = null,
            string? name = null,
            string? phoneNumber = null,
            bool requireAuth = false);
    }
}
