namespace PickleChic.WEB.Services.Storage
{
    public interface ILocalStorageService
    {
        Task SetItemAsync<TValue>(string key, TValue value);

        Task<TValue?> GetItemAsync<TValue>(string key);

        Task<string?> GetStringAsync(string key);

        Task RemoveItemAsync(string key);

        Task ClearAsync();

        Task<bool> ContainsKeyAsync(string key);
    }
}
