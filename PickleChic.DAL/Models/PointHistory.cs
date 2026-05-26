using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class PointHistory
    {
        public int Id { get; set; }
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }

        public int Points { get; set; }

        public string TransactionType { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }

    }
}
