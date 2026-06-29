using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;

namespace PickleChic.WEB.Services.Customer
{
    public class AddressService : IAddressService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public AddressService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public async Task<int?> GetCustomerIdAsync()
        {
            if (!await _authStorageService.IsAuthenticatedAsync())
            {
                return null;
            }

            var user = await _authStorageService.GetUserAsync();

            return user?.Id;
        }

        public async Task<ApiResult<List<AddressResponse>>> GetByCustomerIdAsync(int customerId)
        {
            return await _apiProvider.GetAsync<List<AddressResponse>>(
                EndPointConfig.Address.GetByUser(customerId),
                requireAuth: true);
        }

        public async Task<ApiResult<AddressResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<AddressResponse>(
                EndPointConfig.Address.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<List<ProvinceResponse>>> GetProvincesAsync()
        {
            return await _apiProvider.GetAsync<List<ProvinceResponse>>(
                EndPointConfig.Address.Provinces,
                requireAuth: true);
        }

        public async Task<ApiResult<List<DistrictResponse>>> GetDistrictsByProvinceAsync(int provinceId)
        {
            return await _apiProvider.GetAsync<List<DistrictResponse>>(
                EndPointConfig.Address.DistrictsByProvince(provinceId),
                requireAuth: true);
        }

        public async Task<ApiResult<List<WardResponse>>> GetWardsByDistrictAsync(int districtId)
        {
            return await _apiProvider.GetAsync<List<WardResponse>>(
                EndPointConfig.Address.WardsByDistrict(districtId),
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> CreateAsync(AddressCreateRequest request)
        {
            var result = await _apiProvider.PostAsync<AddressCreateRequest, AddressMutationResponse>(
                EndPointConfig.Address.Create,
                request,
                requireAuth: true);

            if (!result.Success)
            {
                return ApiResult<bool>.Fail(result.Message, result.StatusCode);
            }

            if (request.IsDefault && result.Data?.Id > 0)
            {
                await UnsetOtherDefaultsAsync(request.CustomerId, result.Data.Id);
            }

            return ApiResult<bool>.Ok(true, message: "Đã thêm địa chỉ");
        }

        public async Task<ApiResult<bool>> UpdateAsync(AddressUpdateRequest request)
        {
            var result = await _apiProvider.PutAsync<AddressUpdateRequest, AddressMutationResponse>(
                EndPointConfig.Address.Update,
                request,
                requireAuth: true);

            if (!result.Success)
            {
                return ApiResult<bool>.Fail(result.Message, result.StatusCode);
            }

            if (request.IsDefault)
            {
                await UnsetOtherDefaultsAsync(request.CustomerId, request.Id);
            }

            return ApiResult<bool>.Ok(true, message: "Đã cập nhật địa chỉ");
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            var result = await _apiProvider.DeleteAsync<string>(
                EndPointConfig.Address.Delete(id),
                requireAuth: true);

            if (!result.Success)
            {
                return ApiResult<bool>.Fail(result.Message, result.StatusCode);
            }

            return ApiResult<bool>.Ok(true, message: "Đã xóa địa chỉ");
        }

        private async Task UnsetOtherDefaultsAsync(int customerId, int keepId)
        {
            var listResult = await GetByCustomerIdAsync(customerId);

            if (!listResult.Success || listResult.Data is null)
            {
                return;
            }

            foreach (var address in listResult.Data.Where(x => x.Id != keepId && x.IsDefault))
            {
                var updateRequest = new AddressUpdateRequest
                {
                    Id = address.Id,
                    CustomerId = customerId,
                    FullName = address.FullName,
                    PhoneNumber = address.PhoneNumber,
                    DetailInfo = address.DetailInfo,
                    WardId = address.WardId,
                    IsDefault = false,
                    Status = address.Status
                };

                await _apiProvider.PutAsync<AddressUpdateRequest, AddressMutationResponse>(
                    EndPointConfig.Address.Update,
                    updateRequest,
                    requireAuth: true);
            }
        }
    }
}
