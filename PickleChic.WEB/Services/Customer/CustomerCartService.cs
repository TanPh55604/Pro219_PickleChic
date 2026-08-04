using System.Text.Json;
using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Helpers;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;
using PickleChic.WEB.Services.Storage;

namespace PickleChic.WEB.Services.Customer
{
    public class CustomerCartService : ICustomerCartService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;
        private readonly ILocalStorageService _localStorageService;

        public event Action? CartChanged;

        public CustomerCartService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService,
            ILocalStorageService localStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
            _localStorageService = localStorageService;
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
                var guestItems = await LoadGuestCartAsync();
                return ApiResult<IReadOnlyList<CartLineModel>>.Ok(guestItems);
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

        public async Task<ApiResult<bool>> AddItemAsync(
            int productVariantId,
            int quantity = 1,
            GuestCartItemInfo? guestInfo = null)
        {
            if (quantity < 1)
            {
                return ApiResult<bool>.Fail("Số lượng không hợp lệ");
            }

            var customerId = await GetCustomerIdAsync();

            if (customerId is null)
            {
                return await AddGuestItemAsync(productVariantId, quantity, guestInfo);
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
            if (newQuantity < 1)
            {
                return await RemoveItemAsync(line.Id);
            }

            if (newQuantity > line.StockQuantity)
            {
                return ApiResult<bool>.Fail("Không đủ số lượng trong kho");
            }

            var customerId = await GetCustomerIdAsync();

            if (customerId is null)
            {
                var items = await LoadGuestCartAsync();
                var existing = items.FirstOrDefault(x => x.Id == line.Id);
                if (existing is null)
                {
                    return ApiResult<bool>.Fail("Không tìm thấy sản phẩm trong giỏ hàng");
                }

                existing.Quantity = newQuantity;
                await SaveGuestCartAsync(items);
                NotifyCartChanged();
                return ApiResult<bool>.Ok(true);
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
                var items = await LoadGuestCartAsync();
                var remaining = items.Where(x => x.Id != cartItemId).ToList();
                if (remaining.Count == items.Count)
                {
                    return ApiResult<bool>.Fail("Không tìm thấy sản phẩm trong giỏ hàng");
                }

                await SaveGuestCartAsync(remaining);
                NotifyCartChanged();
                return ApiResult<bool>.Ok(true);
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

        private async Task<ApiResult<bool>> AddGuestItemAsync(
            int productVariantId,
            int quantity,
            GuestCartItemInfo? guestInfo)
        {
            if (guestInfo is null
                || string.IsNullOrWhiteSpace(guestInfo.ProductName)
                || guestInfo.Price < 0)
            {
                return ApiResult<bool>.Fail("Thiếu thông tin sản phẩm để thêm vào giỏ");
            }

            var stockQuantity = guestInfo.StockQuantity > 0
                ? guestInfo.StockQuantity
                : Math.Max(quantity, 1);

            if (quantity > stockQuantity)
            {
                return ApiResult<bool>.Fail("Không đủ số lượng trong kho");
            }

            var items = await LoadGuestCartAsync();
            var existing = items.FirstOrDefault(x => x.ProductVariantId == productVariantId);
            if (existing is not null)
            {
                var nextQty = existing.Quantity + quantity;
                if (nextQty > stockQuantity)
                {
                    return ApiResult<bool>.Fail("Không đủ số lượng trong kho");
                }

                existing.Quantity = nextQty;
                existing.Price = guestInfo.Price;
                existing.StockQuantity = stockQuantity;
                existing.Name = guestInfo.ProductName;
                existing.Variant = string.IsNullOrWhiteSpace(guestInfo.VariantName)
                    ? existing.Variant
                    : guestInfo.VariantName;
                existing.ImageUrl = MediaUrl.Resolve(guestInfo.ImageUrl) ?? existing.ImageUrl;
            }
            else
            {
                var nextId = items.Count == 0 ? -1 : items.Min(x => x.Id) - 1;
                if (nextId >= 0)
                {
                    nextId = -1;
                }

                items.Add(new CartLineModel
                {
                    Id = nextId,
                    CustomerId = -1,
                    ProductVariantId = productVariantId,
                    ProductId = guestInfo.ProductId,
                    Name = guestInfo.ProductName,
                    Variant = string.IsNullOrWhiteSpace(guestInfo.VariantName)
                        ? string.Empty
                        : guestInfo.VariantName,
                    Price = guestInfo.Price,
                    Quantity = quantity,
                    StockQuantity = stockQuantity,
                    ImageUrl = MediaUrl.Resolve(guestInfo.ImageUrl),
                    ProductUrl = guestInfo.ProductId > 0
                        ? RouterConfig.BuildRoute(RouterConfig.Customer.ProductDetail, guestInfo.ProductId)
                        : RouterConfig.Customer.Products
                });
            }

            await SaveGuestCartAsync(items);
            NotifyCartChanged();
            return ApiResult<bool>.Ok(true, message: "Đã thêm vào giỏ hàng");
        }

        private async Task<List<CartLineModel>> LoadGuestCartAsync()
        {
            try
            {
                var items = await _localStorageService.GetItemAsync<List<CartLineModel>>(PickleChic.WEB.Constant.Constant.GuestCart.StorageKey);
                return items ?? new List<CartLineModel>();
            }
            catch
            {
                return new List<CartLineModel>();
            }
        }

        private async Task SaveGuestCartAsync(List<CartLineModel> items)
        {
            await _localStorageService.SetItemAsync(PickleChic.WEB.Constant.Constant.GuestCart.StorageKey, items);
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
