using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QandAProject.Models
{
    public class CommentModel
    {
        public string UserName { get; set; }

        public DateTime DateOfCreated { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage ="message too long")]
        public string Content { get; set; }
    }
}