using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface ICustomerAttributeService
    {
        Task<ApiResult<List<CustomerAttributeResponse>>> GetAllAsync();

        Task<ApiResult<List<CustomerAttributeResponse>>> GetAllByCategoryIdAsync(int categoryId);
    }

    public class CustomerAttributeService : ICustomerAttributeService
    {
        private readonly IApiProvider _apiProvider;

        public CustomerAttributeService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<CustomerAttributeResponse>>> GetAllAsync()
        {
            var result = await _apiProvider.GetAsync<List<CustomerAttributeResponse>>(
                EndPointConfig.PublicAttribute.GetAll,
                requireAuth: false);

            if (!result.Success)
            {
                return result;
            }

            var items = (result.Data ?? new List<CustomerAttributeResponse>())
                .OrderBy(a => a.AttributeName)
                .ToList();

            return ApiResult<List<CustomerAttributeResponse>>.Ok(items, statusCode: result.StatusCode);
        }

        public async Task<ApiResult<List<CustomerAttributeResponse>>> GetAllByCategoryIdAsync(int categoryId)
        {
            var result = await _apiProvider.GetAsync<List<CustomerAttributeResponse>>(
                EndPointConfig.PublicAttribute.GetAllByCategoryId(categoryId),
                requireAuth: false);

            if (!result.Success)
            {
                return result;
            }

            var items = (result.Data ?? new List<CustomerAttributeResponse>())
                .OrderBy(a => a.AttributeName)
                .ToList();

            return ApiResult<List<CustomerAttributeResponse>>.Ok(items, statusCode: result.StatusCode);
        }
    }
}
