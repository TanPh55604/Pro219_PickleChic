namespace PickleChic.WEB.Model
{
    public class CustomerAddressFormModel
    {
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string DetailInfo { get; set; } = string.Empty;

        public int? ProvinceId { get; set; }

        public int? DistrictId { get; set; }

        public int? WardId { get; set; }

        public bool IsDefault { get; set; }
    }
}
