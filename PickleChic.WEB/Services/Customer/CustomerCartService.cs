using System.Text.Json;
using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;

namespace PickleChic.WEB.Services.Customer
{
    public class CustomerCartService : ICustomerCartService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public event Action? CartChanged;

        public CustomerCartService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public void NotifyCartChanged()
        {
            CartChanged?.Invoke();
        }

        public async Task<bool> UsesServerCartAsync()
        {
            return await _authStorageService.IsAuthenticatedAsync();
        }

        public async Task<int?> GetCustomerIdAsync()
        {
            if (!await UsesServerCartAsync())
            {
                return null;
            }

            var user = await _authStorageService.GetUserAsync();

            return user?.Id;
        }

        public async Task<ApiResult<IReadOnlyList<CartLineModel>>> GetItemsAsync()
        {
            var customerId = await GetCustomerIdAsync();

            if (customerId is null)
            {
                // Guest cart: trả danh sách rỗng; sau này đọc từ localStorage tại đây.
                return ApiResult<IReadOnlyList<CartLineModel>>.Ok(Array.Empty<CartLineModel>());
            }

            var result = await _apiProvider.GetAsync<List<CartItemResponse>>(
                EndPointConfig.Cart.GetByUser(customerId.Value),
                requireAuth: true);

            if (!result.Success)
            {
                return ApiResult<IReadOnlyList<CartLineModel>>.Fail(
                    message: result.Message,
                    statusCode: result.StatusCode);
            }

            var lines = (result.Data ?? new List<CartItemResponse>())
                .Select(CartLineModel.FromResponse)
                .ToList();

            return ApiResult<IReadOnlyList<CartLineModel>>.Ok(lines);
        }

        public async Task<int> GetTotalQuantityAsync()
        {
            var result = await GetItemsAsync();

            if (!result.Success || result.Data is null)
            {
                return 0;
            }

            return result.Data.Sum(x => x.Quantity);
        }

        public async Task<ApiResult<bool>> AddItemAsync(int productVariantId, int quantity = 1)
        {
            var customerId = await GetCustomerIdAsync();

            if (customerId is null)
            {
                return ApiResult<bool>.Fail(
                    message: "Vui lòng đăng nhập để thêm vào giỏ hàng",
                    statusCode: 401);
            }

            if (quantity < 1)
            {
                return ApiResult<bool>.Fail("Số lượng không hợp lệ");
            }

            var request = new CartItemCreateRequest
            {
                CustomerId = customerId.Value,
                ProductVariantId = productVariantId,
                Quantity = quantity
            };

            var result = await _apiProvider.PostAsync<CartItemCreateRequest, CartItemResponse>(
                EndPointConfig.Cart.Create,
                request,
                requireAuth: true);

            if (!result.Success)
            {
                return ApiResult<bool>.Fail(
                    message: NormalizeErrorMessage(result.Message),
                    statusCode: result.StatusCode);
            }

            NotifyCartChanged();
            return ApiResult<bool>.Ok(true, message: "Đã thêm vào giỏ hàng");
        }

        public async Task<ApiResult<bool>> UpdateQuantityAsync(CartLineModel line, int newQuantity)
        {
            var customerId = await GetCustomerIdAsync();

            if (customerId is null)
            {
                return ApiResult<bool>.Fail(
                    message: "Vui lòng đăng nhập để cập nhật giỏ hàng",
                    statusCode: 401);
            }

            if (newQuantity < 1)
            {
                return await RemoveItemAsync(line.Id);
            }

            if (newQuantity > line.StockQuantity)
            {
                return ApiResult<bool>.Fail("Không đủ số lượng trong kho");
            }

            var request = new CartItemUpdateRequest
            {
                Id = line.Id,
                CustomerId = customerId.Value,
                ProductVariantId = line.ProductVariantId,
                Quantity = newQuantity
            };

            var result = await _apiProvider.PatchAsync<CartItemUpdateRequest, CartItemResponse>(
                EndPointConfig.Cart.Update,
                request,
                requireAuth: true);

            if (!result.Success)
            {
                return ApiResult<bool>.Fail(
                    message: NormalizeErrorMessage(result.Message),
                    statusCode: result.StatusCode);
            }

            NotifyCartChanged();
            return ApiResult<bool>.Ok(true);
        }

        public async Task<ApiResult<bool>> RemoveItemAsync(int cartItemId)
        {
            var customerId = await GetCustomerIdAsync();

            if (customerId is null)
            {
                return ApiResult<bool>.Fail(
                    message: "Vui lòng đăng nhập để xóa sản phẩm khỏi giỏ hàng",
                    statusCode: 401);
            }

            var result = await _apiProvider.DeleteAsync<string>(
                EndPointConfig.Cart.Delete(cartItemId),
                requireAuth: true);

            if (!result.Success)
            {
                return ApiResult<bool>.Fail(
                    message: NormalizeErrorMessage(result.Message),
                    statusCode: result.StatusCode);
            }

            NotifyCartChanged();
            return ApiResult<bool>.Ok(true);
        }

        private static string NormalizeErrorMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Có lỗi xảy ra";
            }

            message = message.Trim();

            if (!message.StartsWith('{'))
            {
                return message;
            }

            try
            {
                using var doc = JsonDocument.Parse(message);

                if (doc.RootElement.TryGetProperty("message", out var property))
                {
                    var parsed = property.GetString();

                    if (!string.IsNullOrWhiteSpace(parsed))
                    {
                        return parsed;
                    }
                }
            }
            catch
            {
                // Giữ nguyên message gốc.
            }

            return message;
        }
    }
}
