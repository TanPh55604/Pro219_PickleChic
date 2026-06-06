namespace PickleChic.API.DTOs;

public class AddressCreateDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string WardCode { get; set; } = null!;
    public string DistrictCode { get; set; } = null!;
    public string ProvinceCode { get; set; } = null!;
    public string DetailInfo { get; set; } = null!;
    public string WardName { get; set; } = null!;
    public string DistrictName { get; set; } = null!;
    public string ProvinceName { get; set; } = null!;
    public bool IsDefault { get; set; }
    public int Status { get; set; }
    public string? UpdateBy { get; set; }
}

public class AddressUpdateDto : AddressCreateDto
{
    public int Id { get; set; }
}
