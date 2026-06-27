using PickleChic.WEB.Constant;
using PickleChic.WEB.DTO.Admin;
using PickleChic.WEB.Services.Api;

namespace PickleChic.WEB.Services.Admin;

public interface IProductVariantImageService
{
    Task<ApiResult<List<ProductVariantImageResponse>>> GetByVariantIdAsync(int variantId);

    Task<ApiResult<ProductVariantImageResponse>> UploadAsync(
        int variantId,
        Stream fileStream,
        string fileName,
        bool isMain = false);

    Task<ApiResult<ProductVariantImageResponse>> SetMainAsync(int imageId);

    Task<ApiResult<bool>> DeleteAsync(int imageId);
}

public class ProductVariantImageService : IProductVariantImageService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private readonly IApiProvider _apiProvider;

    public ProductVariantImageService(IApiProvider apiProvider)
    {
        _apiProvider = apiProvider;
    }

    public async Task<ApiResult<List<ProductVariantImageResponse>>> GetByVariantIdAsync(int variantId)
    {
        return await _apiProvider.GetAsync<List<ProductVariantImageResponse>>(
            EndPointConfig.ProductVariantImage.GetByVariant(variantId),
            requireAuth: true);
    }

    public async Task<ApiResult<ProductVariantImageResponse>> UploadAsync(
        int variantId,
        Stream fileStream,
        string fileName,
        bool isMain = false)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(fileStream), "file", fileName);
        content.Add(new StringContent(variantId.ToString()), "productVariantId");
        content.Add(new StringContent(isMain.ToString().ToLowerInvariant()), "isMain");

        return await _apiProvider.PostMultipartAsync<ProductVariantImageResponse>(
            EndPointConfig.ProductVariantImage.Upload,
            content,
            requireAuth: true);
    }

    public async Task<ApiResult<ProductVariantImageResponse>> SetMainAsync(int imageId)
    {
        return await _apiProvider.PatchAsync<object, ProductVariantImageResponse>(
            EndPointConfig.ProductVariantImage.SetMain(imageId),
            new { },
            requireAuth: true);
    }

    public async Task<ApiResult<bool>> DeleteAsync(int imageId)
    {
        return await _apiProvider.DeleteAsync<bool>(
            EndPointConfig.ProductVariantImage.Delete(imageId),
            requireAuth: true);
    }

    public static long GetMaxFileSizeBytes() => MaxFileSizeBytes;
}
