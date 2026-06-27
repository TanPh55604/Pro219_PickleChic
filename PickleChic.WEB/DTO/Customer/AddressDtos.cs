namespace PickleChic.WEB.DTO.Customer
{
    public class AddressResponse
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string DetailInfo { get; set; } = string.Empty;

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

        public string DisplayLocation =>
            string.Join(", ",
                new[] { DetailInfo, WardName, DistrictName, ProvinceName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public class AddressCreateRequest
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string DetailInfo { get; set; } = string.Empty;

        public int WardId { get; set; }

        public bool IsDefault { get; set; }

        public int Status { get; set; } = 1;
    }

    public class AddressUpdateRequest : AddressCreateRequest
    {
        public int Id { get; set; }
    }

    public class ProvinceResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }

    public class DistrictResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public int ProvinceId { get; set; }
    }

    public class WardResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public int DistrictId { get; set; }
    }

    public class AddressMutationResponse
    {
        public int Id { get; set; }
    }
}
