using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        //Customer
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public string WardCode { get; set; }
        public string DistrictCode { get; set; }

        public string ProvinceCode { get; set; }

        public string DetailInfo { get; set; }

        public string WardName { get; set; }
        public string DistrictName { get; set; }
        public string ProvinceName { get; set; }
        public bool IsDefault { get; set; }
        public string UpdateBy  { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreateAt { get; set; }

        public DateTime DeleteAt { get; set; }
        public bool Delete { get; set; } = false;
    }
}
