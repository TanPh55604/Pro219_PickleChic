namespace PickleChic.WEB.Helpers
{
    public static class MediaUrl
    {
        public static string ApiBaseUrl { get; set; } = "https://localhost:7001";

        public static string? Resolve(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("blob:", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            return $"{ApiBaseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
        }
    }
}
