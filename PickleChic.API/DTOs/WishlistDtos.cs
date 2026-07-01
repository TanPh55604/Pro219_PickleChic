namespace PickleChic.API.DTOs;

public class WishlistCreateDto
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
}

public class WishlistUpdateDto : WishlistCreateDto
{
    public int Id { get; set; }
}
