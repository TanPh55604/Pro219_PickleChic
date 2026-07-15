namespace PickleChic.API.DTOs;

public class PointHistoryCreateDto
{
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public int Points { get; set; }
    public string TransactionType { get; set; } = null!;
    public string? Description { get; set; }
}

public class PointHistoryUpdateDto : PointHistoryCreateDto
{
    public int Id { get; set; }
}
