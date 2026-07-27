using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;

namespace PickleChic.WEB.Services.Customer
{
    public interface ICustomerReviewService
    {
        Task<ApiResult<List<ReviewResponse>>> GetByVariantAsync(int productVariantId);

        Task<ApiResult<ReviewEligibilityResponse>> GetEligibilityAsync(int productVariantId);

        Task<ApiResult<ReviewResponse>> CreateAsync(ReviewCreateRequest request);

        Task<ApiResult<List<UnreviewedProductVariantResponse>>> GetUnreviewedAsync();
    }

    public class CustomerReviewService : ICustomerReviewService
    {
        private readonly IApiProvider _apiProvider;
        private readonly IAuthStorageService _authStorageService;

        public CustomerReviewService(
            IApiProvider apiProvider,
            IAuthStorageService authStorageService)
        {
            _apiProvider = apiProvider;
            _authStorageService = authStorageService;
        }

        public async Task<ApiResult<List<ReviewResponse>>> GetByVariantAsync(int productVariantId)
        {
            return await _apiProvider.GetAsync<List<ReviewResponse>>(
                EndPointConfig.Review.ByVariant(productVariantId),
                requireAuth: false);
        }

        public async Task<ApiResult<ReviewEligibilityResponse>> GetEligibilityAsync(int productVariantId)
        {
            // Always attach token when present so AllowAnonymous eligibility can resolve the user.
            var isAuthenticated = await _authStorageService.IsAuthenticatedAsync();

            return await _apiProvider.GetAsync<ReviewEligibilityResponse>(
                EndPointConfig.Review.Eligibility(productVariantId),
                requireAuth: isAuthenticated);
        }

        public async Task<ApiResult<ReviewResponse>> CreateAsync(ReviewCreateRequest request)
        {
            return await _apiProvider.PostAsync<ReviewCreateRequest, ReviewResponse>(
                EndPointConfig.Review.Create,
                request,
                requireAuth: true);
        }

        public async Task<ApiResult<List<UnreviewedProductVariantResponse>>> GetUnreviewedAsync()
        {
            return await _apiProvider.GetAsync<List<UnreviewedProductVariantResponse>>(
                EndPointConfig.Review.Unreviewed,
                requireAuth: true);
        }
    }
}
