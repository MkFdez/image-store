using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace DataAccess.Models
{
    public class PublicationViewModel
    {
        public int id { get; set; }
        public string User { get; set; }
        public string ProfilePicture { get; set; }
        public decimal Price { get; set; }
        public int DaysSinceCreated { get; set; }
        public string Content { get; set; }
        public string headerPath { get; set; }
        public List<CommentModel> Comments { get; set; }

        public List<string> Categories { get; set; }

        public string Status { get; set; }

        public bool isBuyed { get; set; }
    }
}