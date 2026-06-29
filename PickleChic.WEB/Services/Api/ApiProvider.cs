using PickleChic.WEB.Services.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PickleChic.WEB.Services.Api
{
    public class ApiProvider : IApiProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthStorageService _authStorageService;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiProvider(
            HttpClient httpClient,
            IAuthStorageService authStorageService)
        {
            _httpClient = httpClient;
            _authStorageService = authStorageService;
        }

        public async Task<ApiResult<TResponse>> GetAsync<TResponse>(
            string url,
            bool requireAuth = false)
        {
            try
            {
                await AddAuthorizationHeaderAsync(requireAuth);

                var response = await _httpClient.GetAsync(url);

                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Fail(
                    message: ex.Message,
                    statusCode: 0);
            }
        }

        public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
            string url,
            TRequest request,
            bool requireAuth = false)
        {
            try
            {
                await AddAuthorizationHeaderAsync(requireAuth);

                var response = await _httpClient.PostAsJsonAsync(url, request, _jsonOptions);

                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Fail(
                    message: ex.Message,
                    statusCode: 0);
            }
        }

        public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
            string url,
            TRequest request,
            bool requireAuth = false)
        {
            try
            {
                await AddAuthorizationHeaderAsync(requireAuth);

                var response = await _httpClient.PutAsJsonAsync(url, request, _jsonOptions);

                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Fail(
                    message: ex.Message,
                    statusCode: 0);
            }
        }

        public async Task<ApiResult<TResponse>> PatchAsync<TRequest, TResponse>(
            string url,
            TRequest request,
            bool requireAuth = false)
        {
            try
            {
                await AddAuthorizationHeaderAsync(requireAuth);

                var content = JsonContent.Create(request, options: _jsonOptions);

                var response = await _httpClient.PatchAsync(url, content);

                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Fail(
                    message: ex.Message,
                    statusCode: 0);
            }
        }

        public async Task<ApiResult<TResponse>> DeleteAsync<TResponse>(
            string url,
            bool requireAuth = false)
        {
            try
            {
                await AddAuthorizationHeaderAsync(requireAuth);

                var response = await _httpClient.DeleteAsync(url);

                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Fail(
                    message: ex.Message,
                    statusCode: 0);
            }
        }

        public async Task<ApiResult<TResponse>> PostMultipartAsync<TResponse>(
            string url,
            MultipartFormDataContent content,
            bool requireAuth = false)
        {
            try
            {
                await AddAuthorizationHeaderAsync(requireAuth);

                var response = await _httpClient.PostAsync(url, content);

                return await ReadResponseAsync<TResponse>(response);
            }
            catch (Exception ex)
            {
                return ApiResult<TResponse>.Fail(
                    message: ex.Message,
                    statusCode: 0);
            }
        }

        private async Task AddAuthorizationHeaderAsync(bool requireAuth)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!requireAuth)
            {
                return;
            }

            var token = await _authStorageService.GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<ApiResult<TResponse>> ReadResponseAsync<TResponse>(
            HttpResponseMessage response)
        {
            var statusCode = (int)response.StatusCode;
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = ExtractErrorMessage(content);

                return ApiResult<TResponse>.Fail(
                    message: errorMessage,
                    statusCode: statusCode);
            }

            if (typeof(TResponse) == typeof(string))
            {
                return ApiResult<TResponse>.Ok(
                    data: (TResponse)(object)content,
                    statusCode: statusCode);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return ApiResult<TResponse>.Ok(
                    data: default,
                    statusCode: statusCode);
            }

            try
            {
                var data = JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);

                return ApiResult<TResponse>.Ok(
                    data: data,
                    statusCode: statusCode);
            }
            catch
            {
                return ApiResult<TResponse>.Fail(
                    message: "Không thể đọc dữ liệu trả về từ server",
                    statusCode: statusCode);
            }
        }

        private static string ExtractErrorMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "Có lỗi xảy ra";
            }

            content = content.Trim();

            if (content.StartsWith("\"") && content.EndsWith("\""))
            {
                try
                {
                    var message = JsonSerializer.Deserialize<string>(content);

                    return string.IsNullOrWhiteSpace(message)
                        ? "Có lỗi xảy ra"
                        : message;
                }
                catch
                {
                    return content.Trim('"');
                }
            }

            return content;
        }
    }
}