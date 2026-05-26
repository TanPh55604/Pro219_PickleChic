using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        public string VoucherCode { get; set; }
        public string VoucherName { get; set; }

        public string VoucherType { get; set; }


    }
}
