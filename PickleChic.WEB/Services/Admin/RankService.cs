using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public class RankService : IRankService
    {
        private readonly IApiProvider _apiProvider;

        public RankService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<RankResponse>>> GetAllAsync(string? keyword = null)
        {
            var url = EndPointConfig.Rank.GetAll;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"?keyword={Uri.EscapeDataString(keyword.Trim())}";
            }

            return await _apiProvider.GetAsync<List<RankResponse>>(url, requireAuth: true);
        }

        public async Task<ApiResult<RankResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<RankResponse>(
                EndPointConfig.Rank.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<RankResponse>> CreateAsync(RankCreateRequest request)
        {
            request.RankName = request.RankName.Trim();

            return await _apiProvider.PostAsync<RankCreateRequest, RankResponse>(
                EndPointConfig.Rank.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<RankResponse>> UpdateAsync(RankUpdateRequest request)
        {
            request.RankName = request.RankName.Trim();

            return await _apiProvider.PatchAsync<RankUpdateRequest, RankResponse>(
                EndPointConfig.Rank.Update,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Rank.Delete(id),
                requireAuth: true);
        }

        public async Task<ApiResult<PercentRewardResponse>> GetPercentRewardAsync()
        {
            return await _apiProvider.GetAsync<PercentRewardResponse>(
                EndPointConfig.Rank.PercentReward,
                requireAuth: true);
        }

        public async Task<ApiResult<PercentRewardResponse>> UpdatePercentRewardAsync(double value)
        {
            return await _apiProvider.PatchAsync<PercentRewardUpdateRequest, PercentRewardResponse>(
                EndPointConfig.Rank.PercentReward,
                new PercentRewardUpdateRequest { PercentReward = value },
                requireAuth: true);
        }
    }
}
