using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string VariantImageUrl { get; set; }
        public int Status { get; set; }
        public TimeSpan RowVersion { get; set; }

    }
}
