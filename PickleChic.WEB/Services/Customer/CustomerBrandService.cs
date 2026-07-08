using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public class CustomerBrandService : ICustomerBrandService
    {
        private readonly IApiProvider _apiProvider;

        public CustomerBrandService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<CustomerBrandResponse>>> GetAllAsync()
        {
            var result = await _apiProvider.GetAsync<List<CustomerBrandResponse>>(
                EndPointConfig.PublicBrand.GetAll,
                requireAuth: false);

            if (!result.Success)
            {
                return result;
            }

            var items = (result.Data ?? new List<CustomerBrandResponse>())
                .OrderBy(b => b.Name)
                .ToList();

            return ApiResult<List<CustomerBrandResponse>>.Ok(items, statusCode: result.StatusCode);
        }

        public async Task<ApiResult<CustomerBrandResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<CustomerBrandResponse>(
                EndPointConfig.PublicBrand.GetById(id),
                requireAuth: false);
        }
    }
}
