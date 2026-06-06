using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Auth
{
    public interface ICurrentUserService
    {
        Task<ApiResult<AuthModel>> CheckCurrentUserAsync();
    }
}