using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class AttributeService : IAttributeService
    {
        private readonly IApiProvider _apiProvider;

        public AttributeService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<AttributeResponse>>> GetAllAsync(string? keyword = null)
        {
            var url = EndPointConfig.Attribute.GetAll;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword.Trim())}";
            }

            return await _apiProvider.GetAsync<List<AttributeResponse>>(
                url,
                requireAuth: true);
        }

        public async Task<ApiResult<AttributeResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<AttributeResponse>(
                EndPointConfig.Attribute.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<AttributeResponse>> CreateAsync(AttributeModel model)
        {
            var request = new AttributeCreateRequest
            {
                AttributeName = model.AttributeName,
                AttributeValues = model.AttributeValues
                    .Where(v => !string.IsNullOrWhiteSpace(v.Value))
                    .Select(v => new AttributeValueItemRequest
                    {
                        Value = v.Value.Trim(),
                        Note = v.Note?.Trim()
                    })
                    .ToList()
            };

            return await _apiProvider.PostAsync<AttributeCreateRequest, AttributeResponse>(
                EndPointConfig.Attribute.CreateWithValues,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<AttributeResponse>> UpdateAsync(AttributeModel model)
        {
            var request = new AttributeUpdateRequest
            {
                Id = model.Id,
                AttributeName = model.AttributeName
            };

            return await _apiProvider.PatchAsync<AttributeUpdateRequest, AttributeResponse>(
                EndPointConfig.Attribute.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Attribute.Delete(id),
                requireAuth: true);
        }

        public async Task<ApiResult<AttributeValueResponse>> CreateValueAsync(AttributeValueModel model)
        {
            var request = new AttributeValueCreateRequest
            {
                AttributeId = model.AttributeId,
                Value = model.Value.Trim(),
                Note = model.Note?.Trim()
            };

            return await _apiProvider.PostAsync<AttributeValueCreateRequest, AttributeValueResponse>(
                EndPointConfig.AttributeValue.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<AttributeValueResponse>> UpdateValueAsync(AttributeValueModel model)
        {
            var request = new AttributeValueUpdateRequest
            {
                Id = model.Id,
                AttributeId = model.AttributeId,
                Value = model.Value.Trim(),
                Note = model.Note?.Trim()
            };

            return await _apiProvider.PatchAsync<AttributeValueUpdateRequest, AttributeValueResponse>(
                EndPointConfig.AttributeValue.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteValueAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.AttributeValue.Delete(id),
                requireAuth: true);
        }
    }
}
