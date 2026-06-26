using PickleChic.WEB.Constant;

namespace PickleChic.WEB.Model
{
    public class ProductSearchQuery
    {
        public string? Keyword { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; }

        public int PageNumber { get; set; } = ProductSearchDefaults.DefaultPageNumber;

        public int PageSize { get; set; } = ProductSearchDefaults.DefaultPageSize;
    }
}
