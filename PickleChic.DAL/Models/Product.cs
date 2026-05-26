using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }

        public string Description { get; set; }

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        [ForeignKey("BrandId")]
        public int BrandId { get; set; }

        public int Status { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdateBy  { get; set; }
        public bool Delete {  get; set; }
    }
}
