using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PickleChic.DAL.Models
{
    public class Staff
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PasswordHash { get; set; }

        [ForeignKey("RoleId")]

        public int RoleId { get; set; }
        public DateTime Lastlogin { get; set; }

        public int Status { get; set; }
    }
}
