using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }



    }
}
