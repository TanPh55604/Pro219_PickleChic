using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public class PointHistoryService : IPointHistoryService
    {
        private readonly IApiProvider _apiProvider;

        public PointHistoryService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<PointHistoryResponse>>> GetByCustomerAsync(int customerId)
        {
            return await _apiProvider.GetAsync<List<PointHistoryResponse>>(
                EndPointConfig.PointHistory.GetByCustomer(customerId),
                requireAuth: true);
        }
    }
}
