using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IOrderService
    {
        Task<ApiResult<List<OrderResponse>>> GetAllAsync(
            string? keyword = null,
            IEnumerable<int>? status = null,
            bool? guestOrder = null,
            bool? isPos = null);

        Task<ApiResult<OrderResponse>> GetByIdAsync(int id);

        Task<ApiResult<OrderResponse>> UpdateStatusAsync(int id, OrderStatusUpdateRequest request);
    }
}
