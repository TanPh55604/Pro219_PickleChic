namespace PickleChic.API.Options;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string UploadRoot { get; set; } = "uploads/products";

    public string CategoryUploadRoot { get; set; } = "uploads/categories";

    public int MaxFileSizeMb { get; set; } = 5;

    public int MaxImagesPerVariant { get; set; } = 6;

    public string PublicBaseUrl { get; set; } = "https://localhost:7001";
}
