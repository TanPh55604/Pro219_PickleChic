using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IAttributeService
    {
        Task<ApiResult<List<AttributeResponse>>> GetAllAsync(string? keyword = null);

        Task<ApiResult<AttributeResponse>> GetByIdAsync(int id);

        Task<ApiResult<AttributeResponse>> CreateAsync(AttributeModel model);

        Task<ApiResult<AttributeResponse>> UpdateAsync(AttributeModel model);

        Task<ApiResult<bool>> DeleteAsync(int id);

        Task<ApiResult<AttributeValueResponse>> CreateValueAsync(AttributeValueModel model);

        Task<ApiResult<AttributeValueResponse>> UpdateValueAsync(AttributeValueModel model);

        Task<ApiResult<bool>> DeleteValueAsync(int id);
    }
}
