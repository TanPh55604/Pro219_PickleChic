namespace PickleChic.WEB.DTO.Admin
{
    public class AdminReviewResponse
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
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? VariantName { get; set; }
        public string? SKU { get; set; }

        public bool IsVisible => Status == 1;
    }

    public class ReviewStatusUpdateRequest
    {
        public int Id { get; set; }
        public int Status { get; set; }
    }
}
