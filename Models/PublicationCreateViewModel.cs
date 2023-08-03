using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class PublicationCreateViewModel
    {
        [Required]
        [MaxLength(100, ErrorMessage ="Description too long")]
        public string Content { get; set; }
        public string HeaderPath { get; set; }

        public List<int> Categories { get; set; }
        [Required]
        public HttpPostedFileBase Picture { get; set; }

        [Required]      
        public string Price { get; set; }
    }
}