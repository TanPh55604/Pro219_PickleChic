using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int TotalPoints { get; set; }

        public DateTime LastLogin { get; set; }

        public int Status { get; set; }

        public ICollection<Address>? Addresses { get; set; }

    }
}
