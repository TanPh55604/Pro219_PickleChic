namespace PickleChic.WEB.DTO.Customer
{
    public class ReviewCreateRequest
    {
        public int ProductVariantId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Overall { get; set; }
    }

    public class ReviewResponse
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Overall { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerUsername { get; set; }
    }

    public class ReviewEligibilityResponse
    {
        public bool IsAuthenticated { get; set; }
        public bool CanReview { get; set; }
        public bool AlreadyReviewed { get; set; }
        public bool HasCompletedPurchase { get; set; }
        public string LockReason { get; set; } = "none";
        public string? MyReviewState { get; set; }
    }

    public class UnreviewedProductVariantResponse
    {
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string? SKU { get; set; }
        public string? VariantName { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
    }
}
