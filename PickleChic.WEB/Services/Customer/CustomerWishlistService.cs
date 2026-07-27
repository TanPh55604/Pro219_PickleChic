using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;

namespace PickleChic.WEB.Services.Customer
{
    public interface ICustomerWishlistService
    {
        Task<ApiResult<List<WishlistProductResponse>>> GetMyProductsAsync();

        Task<ApiResult<List<WishlistItemResponse>>> GetMyEntriesAsync();

        Task<ApiResult<(bool IsFavorite, int? WishlistId)>> GetFavoriteStateAsync(int productId);

        Task<ApiResult<WishlistItemResponse>> AddAsync(int productId);

        Task<ApiResult<bool>> RemoveAsync(int wishlistId);

        Task<ApiResult<(bool IsFavorite, int? WishlistId)>> ToggleAsync(int productId);
    }

    public class CustomerWishlistService : ICustomerWishlistService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public CustomerWishlistService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public async Task<ApiResult<List<WishlistProductResponse>>> GetMyProductsAsync()
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId is null)
            {
                return ApiResult<List<WishlistProductResponse>>.Fail(
                    message: "Vui lòng đăng nhập",
                    statusCode: 401);
            }

            var result = await _apiProvider.GetAsync<List<WishlistProductResponse>>(
                EndPointConfig.Wishlist.GetAllByUserId(customerId.Value),
                requireAuth: true);

            if (result.Success && result.Data is null)
            {
                return ApiResult<List<WishlistProductResponse>>.Ok(new List<WishlistProductResponse>());
            }

            return result;
        }

        public async Task<ApiResult<List<WishlistItemResponse>>> GetMyEntriesAsync()
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId is null)
            {
                return ApiResult<List<WishlistItemResponse>>.Fail(
                    message: "Vui lòng đăng nhập",
                    statusCode: 401);
            }

            // Public list API không trả wishlist Id — dùng management get-all rồi lọc theo user.
            var result = await _apiProvider.GetAsync<List<WishlistItemResponse>>(
                EndPointConfig.Wishlist.ManagementGetAll,
                requireAuth: true);

            if (!result.Success)
            {
                return result;
            }

            var items = (result.Data ?? new List<WishlistItemResponse>())
                .Where(x => x.CustomerId == customerId.Value)
                .ToList();

            return ApiResult<List<WishlistItemResponse>>.Ok(items);
        }

        public async Task<ApiResult<(bool IsFavorite, int? WishlistId)>> GetFavoriteStateAsync(int productId)
        {
            var entriesResult = await GetMyEntriesAsync();
            if (!entriesResult.Success)
            {
                return ApiResult<(bool, int?)>.Fail(
                    message: entriesResult.Message,
                    statusCode: entriesResult.StatusCode);
            }

            var entry = (entriesResult.Data ?? new List<WishlistItemResponse>())
                .FirstOrDefault(x => x.ProductId == productId);

            return ApiResult<(bool, int?)>.Ok((entry is not null, entry?.Id));
        }

        public async Task<ApiResult<WishlistItemResponse>> AddAsync(int productId)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId is null)
            {
                return ApiResult<WishlistItemResponse>.Fail(
                    message: "Vui lòng đăng nhập",
                    statusCode: 401);
            }

            var existing = await GetFavoriteStateAsync(productId);
            if (existing.Success && existing.Data.IsFavorite && existing.Data.WishlistId.HasValue)
            {
                return ApiResult<WishlistItemResponse>.Ok(new WishlistItemResponse
                {
                    Id = existing.Data.WishlistId.Value,
                    CustomerId = customerId.Value,
                    ProductId = productId
                });
            }

            return await _apiProvider.PostAsync<WishlistCreateRequest, WishlistItemResponse>(
                EndPointConfig.Wishlist.Create,
                new WishlistCreateRequest
                {
                    CustomerId = customerId.Value,
                    ProductId = productId
                },
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> RemoveAsync(int wishlistId)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Wishlist.Delete(wishlistId),
                requireAuth: true);
        }

        public async Task<ApiResult<(bool IsFavorite, int? WishlistId)>> ToggleAsync(int productId)
        {
            var state = await GetFavoriteStateAsync(productId);
            if (!state.Success)
            {
                return state;
            }

            if (state.Data.IsFavorite && state.Data.WishlistId.HasValue)
            {
                var remove = await RemoveAsync(state.Data.WishlistId.Value);
                if (!remove.Success)
                {
                    return ApiResult<(bool, int?)>.Fail(
                        message: remove.Message,
                        statusCode: remove.StatusCode);
                }

                return ApiResult<(bool, int?)>.Ok((false, null));
            }

            var add = await AddAsync(productId);
            if (!add.Success || add.Data is null)
            {
                return ApiResult<(bool, int?)>.Fail(
                    message: add.Message,
                    statusCode: add.StatusCode);
            }

            return ApiResult<(bool, int?)>.Ok((true, add.Data.Id));
        }

        private async Task<int?> GetCustomerIdAsync()
        {
            if (!await _authStorageService.IsAuthenticatedAsync())
            {
                return null;
            }

            var user = await _authStorageService.GetUserAsync();
            return user is { Id: > 0 } ? user.Id : null;
        }
    }
}
