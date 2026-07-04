namespace PickleChic.API.DTOs;

public class PaymentMethodCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class PaymentMethodUpdateDto : PaymentMethodCreateDto
{
    public int Id { get; set; }
}
