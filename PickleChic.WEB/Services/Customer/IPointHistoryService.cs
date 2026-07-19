using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface IPointHistoryService
    {
        Task<ApiResult<List<PointHistoryResponse>>> GetByCustomerAsync(int customerId);
    }
}
