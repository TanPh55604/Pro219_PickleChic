using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface ICustomerBrandService
    {
        Task<ApiResult<List<CustomerBrandResponse>>> GetAllAsync();

        Task<ApiResult<CustomerBrandResponse>> GetByIdAsync(int id);
    }
}
