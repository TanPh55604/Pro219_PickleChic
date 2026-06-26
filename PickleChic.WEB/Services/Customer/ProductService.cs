using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Customer;
using PickleChic.WEB.Model;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Customer
{
    public class ProductService : IProductService
    {
        private readonly IApiProvider _apiProvider;

        public ProductService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<ApiResult<ProductVariantSearchPageResponse>> SearchAsync(ProductSearchQuery query)
        {
            var url = BuildSearchUrl(query);

            return await _apiProvider.GetAsync<ProductVariantSearchPageResponse>(
                url,
                requireAuth: false);
        }

        private static string BuildSearchUrl(ProductSearchQuery query)
        {
            var parameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                parameters.Add($"keyword={Uri.EscapeDataString(query.Keyword.Trim())}");
            }

            if (query.MinPrice.HasValue)
            {
                parameters.Add($"startingPrice={query.MinPrice.Value}");
            }

            if (query.MaxPrice.HasValue)
            {
                parameters.Add($"toPrice={query.MaxPrice.Value}");
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                parameters.Add($"sortBy={Uri.EscapeDataString(query.SortBy)}");
            }

            if (query.CategoryId.HasValue)
            {
                parameters.Add($"categoryId={query.CategoryId.Value}");
            }

            if (query.BrandId.HasValue)
            {
                parameters.Add($"brandId={query.BrandId.Value}");
            }

            parameters.Add($"pageNumber={query.PageNumber}");
            parameters.Add($"pageSize={query.PageSize}");

            return $"{EndPointConfig.Product.Search}?{string.Join("&", parameters)}";
        }
    }
}
