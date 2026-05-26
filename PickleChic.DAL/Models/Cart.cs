using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }

        public int Status   { get; set; }


    }
}
