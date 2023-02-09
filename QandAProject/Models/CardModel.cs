using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Validation;

namespace QandAProject.Models
{
    public class CardModel
    {
        [Required]
        //[ValidCardSecPin]
        public string Number { get; set; }
        [Required]
        //[ValidCardDate]
        public int ExpMont { get; set; }
        [Required]
       // [ValidCardDate]
        public int ExpYear { get; set; }
        [Required]
        //[ValidCardSecPin]
        public int SaveCode { get; set; }
    }
}