namespace PickleChic.WEB.Services.Api
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }

        public T? Data { get; set; }

        public string Message { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public static ApiResult<T> Ok(T? data, string message = "Success", int statusCode = 200)
        {
            return new ApiResult<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ApiResult<T> Fail(string message, int statusCode = 400)
        {
            return new ApiResult<T>
            {
                Success = false,
                Data = default,
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}