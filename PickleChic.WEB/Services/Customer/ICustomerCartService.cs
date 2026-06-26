using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public interface ICustomerCartService
    {
        event Action? CartChanged;

        Task<bool> UsesServerCartAsync();

        Task<int?> GetCustomerIdAsync();

        Task<ApiResult<IReadOnlyList<CartLineModel>>> GetItemsAsync();

        Task<int> GetTotalQuantityAsync();

        Task<ApiResult<bool>> AddItemAsync(int productVariantId, int quantity = 1);

        Task<ApiResult<bool>> UpdateQuantityAsync(CartLineModel line, int newQuantity);

        Task<ApiResult<bool>> RemoveItemAsync(int cartItemId);

        void NotifyCartChanged();
    }
}
