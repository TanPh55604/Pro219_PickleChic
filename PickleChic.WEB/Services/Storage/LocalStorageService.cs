using System.Text.Json;
using Microsoft.JSInterop;

namespace PickleChic.WEB.Services.Storage
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetItemAsync<TValue>(string key, TValue value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("LocalStorage key is required", nameof(key));
            }

            var json = JsonSerializer.Serialize(value, _jsonOptions);

            await _jsRuntime.InvokeVoidAsync(
                "localStorage.setItem",
                key,
                json);
        }

        public async Task<TValue?> GetItemAsync<TValue>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("LocalStorage key is required", nameof(key));
            }

            var json = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                key);

            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<TValue>(json, _jsonOptions);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("LocalStorage key is required", nameof(key));
            }

            var value = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                key);

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<string>(value, _jsonOptions);
            }
            catch
            {
                return value;
            }
        }

        public async Task RemoveItemAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("LocalStorage key is required", nameof(key));
            }

            await _jsRuntime.InvokeVoidAsync(
                "localStorage.removeItem",
                key);
        }

        public async Task ClearAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("LocalStorage key is required", nameof(key));
            }

            var value = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                key);

            return !string.IsNullOrWhiteSpace(value);
        }
    }
}