namespace PickleChic.WEB.DTO.Customer
{
    public class ProductVariantSearchPageResponse
    {
        public List<ProductVariantSearchResponse> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
