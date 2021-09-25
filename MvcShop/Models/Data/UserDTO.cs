using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcShop.Models.Data
{
    [Table("Users")]
    public class UserDTO
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAdress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}