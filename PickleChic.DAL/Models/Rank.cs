using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Rank
    {
        [Key]
        public int Id { get; set; }
        public string RankName { get; set; }
        public string MinPoints { get; set; }
    }
}
