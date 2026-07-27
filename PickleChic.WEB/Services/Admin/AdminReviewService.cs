using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin
{
    public interface IAdminReviewService
    {
        Task<ApiResult<List<AdminReviewResponse>>> GetAllAsync(string? keyword = null, int? status = null);

        Task<ApiResult<AdminReviewResponse>> GetByIdAsync(int id);

        Task<ApiResult<AdminReviewResponse>> UpdateStatusAsync(int id, int status);

        Task<ApiResult<bool>> DeleteAsync(int id);
    }

    public class AdminReviewService : IAdminReviewService
    {
        private readonly IApiProvider _apiProvider;

        public AdminReviewService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<List<AdminReviewResponse>>> GetAllAsync(
            string? keyword = null,
            int? status = null)
        {
            return await _apiProvider.GetAsync<List<AdminReviewResponse>>(
                EndPointConfig.Review.Management.GetAll(keyword, status),
                requireAuth: true);
        }

        public async Task<ApiResult<AdminReviewResponse>> GetByIdAsync(int id)
        {
            return await _apiProvider.GetAsync<AdminReviewResponse>(
                EndPointConfig.Review.Management.GetById(id),
                requireAuth: true);
        }

        public async Task<ApiResult<AdminReviewResponse>> UpdateStatusAsync(int id, int status)
        {
            return await _apiProvider.PatchAsync<ReviewStatusUpdateRequest, AdminReviewResponse>(
                EndPointConfig.Review.Management.UpdateStatus,
                new ReviewStatusUpdateRequest { Id = id, Status = status },
                requireAuth: true);
        }

        public async Task<ApiResult<bool>> DeleteAsync(int id)
        {
            return await _apiProvider.DeleteAsync<bool>(
                EndPointConfig.Review.Management.Delete(id),
                requireAuth: true);
        }
    }
}
