namespace PickleChic.WEB.DTO.Customer
{
    public class PointHistoryResponse
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int OrderId { get; set; }

        public int Points { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
