namespace PickleChic.WEB.Services.Api
{
    public interface IApiProvider
    {
        Task<ApiResult<TResponse>> GetAsync<TResponse>(
            string url,
            bool requireAuth = false);

        Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
            string url,
            TRequest request,
            bool requireAuth = false);

        Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
            string url,
            TRequest request,
            bool requireAuth = false);

        Task<ApiResult<TResponse>> PatchAsync<TRequest, TResponse>(
            string url,
            TRequest request,
            bool requireAuth = false);

        Task<ApiResult<TResponse>> DeleteAsync<TResponse>(
            string url,
            bool requireAuth = false);

        Task<ApiResult<TResponse>> PostMultipartAsync<TResponse>(
            string url,
            MultipartFormDataContent content,
            bool requireAuth = false);
    }
}
