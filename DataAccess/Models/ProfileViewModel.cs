﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Validation;
namespace DataAccess.Models
{
    public class ProfileViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage ="Username too long")]
        //[AlreadyInDB(emailOrUser: 0, not: true, _edit:true)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Email too long")]
        //[AlreadyInDB(emailOrUser:1, not:true, _edit:true)]
        public string Email { get; set; }
       
        
       public string ProfilePicture { get; set; }

        public HttpPostedFileBase PostedPicture { get; set; }

    }
}