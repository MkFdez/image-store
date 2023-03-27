using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
//using Validation;
//Deprecated
namespace QandAProject.Models
{
    public class UserLoginViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage ="Email must have up to 50 characters")]
        //[AlreadyInDB(emailOrUser: 1, not: false)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Password must have up to 50 characters")]
   
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}