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

public class AddressResultDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string DetailInfo { get; set; } = null!;
    public int WardId { get; set; }
    public string? WardName { get; set; }
    public string? WardCode { get; set; }
    public int DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public string? DistrictCode { get; set; }
    public int ProvinceId { get; set; }
    public string? ProvinceName { get; set; }
    public string? ProvinceCode { get; set; }
    public bool IsDefault { get; set; }
    public int Status { get; set; }
    public DateTime InsertedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class FeeItemDTO
{
    public string Name { get; set; } = "Quần áo";
    public int Quantity { get; set; } = 1;
    public int Length { get; set; } = 35;
    public int Width { get; set; } = 35;
    public int Height { get; set; } = 5;
    public int Weight { get; set; } = 500;
}
