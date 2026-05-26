using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class AttributeValue
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public string Note { get; set; }
        
    }
}
