using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface ICustomerCategoryService
    {
        Task<ApiResult<List<CustomerCategoryResponse>>> GetAllAsync();

        Task<ApiResult<CustomerCategoryResponse>> GetByIdAsync(int id);
    }
}
