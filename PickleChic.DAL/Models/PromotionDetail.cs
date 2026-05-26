using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class PromotionDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("PromotionId")]
        public int PromotionId { get; set; }
        [ForeignKey("ProductVariantId")]
        public int ProductVariantId { get; set; }

        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }


    }
}
