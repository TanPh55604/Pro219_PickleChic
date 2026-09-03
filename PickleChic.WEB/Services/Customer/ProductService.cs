using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
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

        public async Task<ApiResult<HomeProductsResponse>> GetHomeProductsAsync()
        {
            return await _apiProvider.GetAsync<HomeProductsResponse>(
                EndPointConfig.Product.Home,
                requireAuth: false);
        }

        public async Task<ApiResult<ProductFilterPageResponse>> FilterAsync(ProductSearchQuery query)
        {
            var url = BuildFilterUrl(query);

            return await _apiProvider.GetAsync<ProductFilterPageResponse>(
                url,
                requireAuth: false);
        }

        public async Task<ApiResult<ProductDetailResponse>> GetByIdWithDetailsAsync(int id)
        {
            return await _apiProvider.GetAsync<ProductDetailResponse>(
                EndPointConfig.Product.GetByIdWithDetailsPublic(id),
                requireAuth: false);
        }

        public async Task<ApiResult<List<ProductVariantSearchResponse>>> GetRelatedByCategoryAsync(
            int categoryId,
            int excludeProductId,
            int limit = 6)
        {
            var result = await _apiProvider.GetAsync<List<ProductVariantSearchResponse>>(
                EndPointConfig.ProductVariant.GetByCategory(categoryId),
                requireAuth: false);

            if (!result.Success || result.Data is null)
            {
                return result;
            }

            var related = result.Data
                .Where(v => v.ProductId != excludeProductId)
                .GroupBy(v => v.ProductId)
                .Select(g => g.First())
                .Take(limit)
                .ToList();

            return ApiResult<List<ProductVariantSearchResponse>>.Ok(related);
        }

        private static string BuildFilterUrl(ProductSearchQuery query)
        {
            var parameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                parameters.Add($"keyword={Uri.EscapeDataString(query.Keyword.Trim())}");
            }

            if (query.MinPrice.HasValue)
            {
                parameters.Add($"startingPrice={query.MinPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }

            if (query.MaxPrice.HasValue)
            {
                parameters.Add($"toPrice={query.MaxPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
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

            foreach (var attributeValueId in query.AttributeValueIds.Distinct().Where(id => id > 0))
            {
                parameters.Add($"attributeValueIds={attributeValueId}");
            }

            parameters.Add($"pageNumber={query.PageNumber}");
            parameters.Add($"pageSize={query.PageSize}");

            return $"{EndPointConfig.Product.Filter}?{string.Join("&", parameters)}";
        }
    }
}
