using MvcShop.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcShop.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM() { }
        public UserVM(UserDTO row)
        {
            Id = row.Id;
            Username = row.Username;
            EmailAdress = row.EmailAdress;
            Password = row.Password;
            FirstName = row.FirstName;
            LastName = row.LastName;
        }

        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAdress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    
    }   
}