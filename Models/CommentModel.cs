using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class CommentModel
    {
        public string UserName { get; set; }

        public int DaysSinceCreated { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage ="message too long")]
        public string Content { get; set; }
        public string ProfilePicture { get; set; }
    }
}