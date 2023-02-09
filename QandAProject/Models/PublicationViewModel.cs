using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace QandAProject.Models
{
    public class PublicationViewModel
    {
        public int id { get; set; }
        public string User { get; set; }
        public string DateOfCreated { get; set; }
        public string Content { get; set; }
        public string headerPath { get; set; }
        public List<CommentModel> Comments { get; set; }

        public List<string> Categories { get; set; }

        public string Status { get; set; }

        public bool isBuyed { get; set; }
    }
}