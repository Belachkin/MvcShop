﻿using MvcShop.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcShop.Models.ViewModels.Account
{
    public class UserProfileVM
    {
        public UserProfileVM() { }
        public UserProfileVM(UserDTO row)
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
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAdress { get; set; }
        
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }       
        public string Password { get; set; }      
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}