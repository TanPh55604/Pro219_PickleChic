using PickleChic.WEB.DTO.Customer;

namespace PickleChic.WEB.DTO.Admin
{
    public class ProductVariantSearchPageResponse
    {
        public List<ProductVariantSearchResponse> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class PosStockCheckResponse
    {
        public int ProductVariantId { get; set; }

        public int StockQuantity { get; set; }

        public int RequestedQuantity { get; set; }

        public bool IsAvailable { get; set; }

        public decimal UnitPrice { get; set; }

        public string? ProductName { get; set; }

        public string? VariantName { get; set; }

        public string? Sku { get; set; }

        public string? Message { get; set; }
    }

    public class CustomerSearchResultResponse
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int TotalPoints { get; set; }

        public int Status { get; set; }

        public int RankId { get; set; }

        public string? RankName { get; set; }

        public DateTime? LastLogin { get; set; }
    }

    public class CustomerSearchPageResponse
    {
        public List<CustomerSearchResultResponse> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public class PosAddressCreateRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string DetailInfo { get; set; } = string.Empty;

        public int WardId { get; set; }
    }

    public class PosOrderCalculationRequest
    {
        public List<OrderItemCalculationRequest> Items { get; set; } = new();

        public int? CustomerId { get; set; }

        public string? DiscountCode { get; set; }

        public int? AddressId { get; set; }

        public bool? UsePoints { get; set; }

        public PosAddressCreateRequest? AddressDTO { get; set; }

        public bool IsShipping { get; set; }
    }

    public class PosCheckoutRequest
    {
        public List<CartItemCheckoutRequest> ListItemCheckout { get; set; } = new();

        public int? CustomerId { get; set; }

        public int? AddressId { get; set; }

        public PosAddressCreateRequest? AddressDTO { get; set; }

        public int? PaymentMethodTypeId { get; set; }

        public int? VoucherId { get; set; }

        public string? Note { get; set; }

        public bool? UsePoints { get; set; } = false;

        public bool IsShipping { get; set; }
    }
}
