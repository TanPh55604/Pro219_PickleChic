using Microsoft.Extensions.Options;
using PickleChic.API.Options;

namespace PickleChic.API.Services;

public class LocalImageFileService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private readonly IWebHostEnvironment _environment;
    private readonly FileStorageOptions _options;

    public LocalImageFileService(
        IWebHostEnvironment environment,
        IOptions<FileStorageOptions> options)
    {
        _environment = environment;
        _options = options.Value;
    }

    public long MaxFileSizeBytes => _options.MaxFileSizeMb * 1024L * 1024L;

    public Task<(string RelativePath, string PublicUrl)> SaveProductVariantImageAsync(
        IFormFile file,
        int productId,
        int variantId)
    {
        var relativeDirectory = Path.Combine(
            _options.UploadRoot,
            productId.ToString(),
            variantId.ToString());

        return SaveAsync(file, relativeDirectory);
    }

    public Task<(string RelativePath, string PublicUrl)> SaveCategoryImageAsync(
        IFormFile file,
        int categoryId)
    {
        var relativeDirectory = Path.Combine(
            _options.CategoryUploadRoot,
            categoryId.ToString());

        return SaveAsync(file, relativeDirectory);
    }

    public void DeleteByPublicUrl(string? url) =>
        DeleteByPublicUrl(url, requiredRootPrefix: null);

    public void DeleteCategoryImageByPublicUrl(string? url) =>
        DeleteByPublicUrl(url, _options.CategoryUploadRoot);

    public string BuildPublicUrl(string relativePath) =>
        $"{_options.PublicBaseUrl.TrimEnd('/')}/{relativePath.Replace('\\', '/')}";

    private async Task<(string RelativePath, string PublicUrl)> SaveAsync(
        IFormFile file,
        string relativeDirectory)
    {
        ValidateFile(file);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absoluteDirectory = Path.Combine(_environment.WebRootPath, relativeDirectory);

        Directory.CreateDirectory(absoluteDirectory);

        var absolutePath = Path.Combine(absoluteDirectory, fileName);
        await using var stream = new FileStream(absolutePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var relativePath = Path.Combine(relativeDirectory, fileName).Replace('\\', '/');
        var publicUrl = BuildPublicUrl(relativePath);

        return (relativePath, publicUrl);
    }

    private void DeleteByPublicUrl(string? url, string? requiredRootPrefix)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        var relativePath = TryGetRelativePath(url);
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(requiredRootPrefix)
            && !relativePath.StartsWith(requiredRootPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var absolutePath = Path.Combine(
            _environment.WebRootPath,
            relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }

    private string? TryGetRelativePath(string url)
    {
        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        if (url.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase))
        {
            return url[baseUrl.Length..].TrimStart('/');
        }

        if (url.StartsWith('/'))
        {
            return url.TrimStart('/');
        }

        return null;
    }

    private void ValidateFile(IFormFile file)
    {
        if (file.Length <= 0)
        {
            throw new InvalidOperationException("File rỗng");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new InvalidOperationException($"Ảnh vượt quá {_options.MaxFileSizeMb}MB");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Chỉ hỗ trợ ảnh JPG, PNG, WEBP");
        }
    }
}
