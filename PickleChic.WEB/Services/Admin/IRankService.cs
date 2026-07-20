using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IRankService
    {
        Task<ApiResult<List<RankResponse>>> GetAllAsync(string? keyword = null);

        Task<ApiResult<RankResponse>> GetByIdAsync(int id);

        Task<ApiResult<RankResponse>> CreateAsync(RankCreateRequest request);

        Task<ApiResult<RankResponse>> UpdateAsync(RankUpdateRequest request);

        Task<ApiResult<bool>> DeleteAsync(int id);

        Task<ApiResult<PercentRewardResponse>> UpdatePercentRewardAsync(double value);
    }
}
