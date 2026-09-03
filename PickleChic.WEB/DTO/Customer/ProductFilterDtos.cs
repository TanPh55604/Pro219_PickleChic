namespace PickleChic.WEB.DTO.Customer
{
    public class ProductFilterPageResponse
    {
        public List<ProductVariantSearchResponse> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class HomeProductsResponse
    {
        public List<ProductVariantSearchResponse> NewProducts { get; set; } = new();

        public List<ProductVariantSearchResponse> BestSellingProducts { get; set; } = new();
    }

    public class CustomerAttributeResponse
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public List<CustomerAttributeValueResponse>? AttributeValues { get; set; }
    }

    public class CustomerAttributeValueResponse
    {
        public int Id { get; set; }

        public int AttributeId { get; set; }

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }

        public string? AttributeName { get; set; }
    }
}
