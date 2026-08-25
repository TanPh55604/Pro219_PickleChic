using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class OrderService : IOrderService
    {
        private readonly IApiProvider _apiProvider;

        public OrderService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<OrderResponse>>> GetAllAsync(
            string? keyword = null,
            IEnumerable<int>? status = null,
            bool? guestOrder = null,
            bool? isPos = null)
        {
            var url = EndPointConfig.Order.GetAll(keyword, status, guestOrder, isPos);

            return await _apiProvider.GetAsync<List<OrderResponse>>(
                url,
                requireAuth: true);
        }

        public async Task<ApiResult<OrderResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<OrderResponse>(
                EndPointConfig.Order.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<OrderResponse>> UpdateStatusAsync(int id, OrderStatusUpdateRequest request)
        {
            return await _apiProvider.PatchAsync<OrderStatusUpdateRequest, OrderResponse>(
                EndPointConfig.Order.UpdateStatus(id),
                request,
                requireAuth: true);
        }
    }
}
