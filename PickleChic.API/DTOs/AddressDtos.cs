namespace PickleChic.API.DTOs;

public class AddressCreateDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string DetailInfo { get; set; } = null!;
    public int WardId { get; set; }
    public bool IsDefault { get; set; }
    public int Status { get; set; }
}

public class AddressUpdateDto : AddressCreateDto
{
    public int Id { get; set; }
}
