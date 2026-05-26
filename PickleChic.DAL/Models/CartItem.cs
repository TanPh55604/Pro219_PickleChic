using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CartId")]
        public int CartId { get; set; }
        [ForeignKey("ProductVariantId")]
        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

    }
}
