using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("ParentCategoryId")]
        public int ParentCategoryId { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateAt { get; set; }
        public  int Status { get; set; }

    }
}
