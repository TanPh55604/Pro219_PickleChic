using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public class CustomerCategoryService : ICustomerCategoryService
    {
        private readonly IApiProvider _apiProvider;

        public CustomerCategoryService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<CustomerCategoryResponse>>> GetAllAsync()
        {
            var result = await _apiProvider.GetAsync<List<CustomerCategoryResponse>>(
                EndPointConfig.PublicCategory.GetAll,
                requireAuth: false);

            if (!result.Success)
            {
                return result;
            }

            var items = (result.Data ?? new List<CustomerCategoryResponse>())
                .OrderBy(c => c.Name)
                .ToList();

            return ApiResult<List<CustomerCategoryResponse>>.Ok(items, statusCode: result.StatusCode);
        }

        public async Task<ApiResult<CustomerCategoryResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<CustomerCategoryResponse>(
                EndPointConfig.PublicCategory.GetById(id),
                requireAuth: false);
        }
    }
}
