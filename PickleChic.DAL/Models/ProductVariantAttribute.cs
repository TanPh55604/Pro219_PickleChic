using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class ProductVariantAttribute
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ProductVariantId")]
        public int ProductVariantId { get; set; }
        [ForeignKey("AttributeValueId")]
        public int AttributeValueId { get; set; }
    }
}
