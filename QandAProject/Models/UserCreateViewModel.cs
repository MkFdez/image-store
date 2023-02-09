using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Validation;
namespace QandAProject.Models
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage ="You must field this entry")]
        [MaxLength(50, ErrorMessage ="username too long")]
        [AlreadyInDB(emailOrUser:0,not:true)]
        public string UserName { get; set; }


        [MaxLength(50, ErrorMessage = "email too long")]
        [Required(ErrorMessage = "You must field this entry")]
        [AlreadyInDB(emailOrUser: 1, not: true)]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "The password must contain from 8 to 50 characters")]
        [MinLength(8, ErrorMessage ="The password must contain from 8 to 50 characters")]
        [Required(ErrorMessage = "You must field this entry")]
        [Compare("RepeatPassword", ErrorMessage ="Password don't match")]
        public string Password { get; set; }


        [MaxLength(50, ErrorMessage = "The password must contain from 8 to 50 characters")]
        [MinLength(8, ErrorMessage = "The password must contain from 8 to 50 characters")]
        [Required(ErrorMessage = "You must field this entry")]
        public string RepeatPassword { get; set; }


    }
}