using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface IAddressService
    {
        Task<int?> GetCustomerIdAsync();

        Task<ApiResult<List<AddressResponse>>> GetByCustomerIdAsync(int customerId);

        Task<ApiResult<AddressResponse>> GetByIdAsync(int id);

        Task<ApiResult<List<ProvinceResponse>>> GetProvincesAsync();

        Task<ApiResult<List<DistrictResponse>>> GetDistrictsByProvinceAsync(int provinceId);

        Task<ApiResult<List<WardResponse>>> GetWardsByDistrictAsync(int districtId);

        Task<ApiResult<bool>> CreateAsync(AddressCreateRequest request);

        Task<ApiResult<bool>> UpdateAsync(AddressUpdateRequest request);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }
}
