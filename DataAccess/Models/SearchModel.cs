using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAccess.Models
{
    public class SearchModel
    {
        public string Search { get; set; }
        public SearchModel()
        {
            Search = "";
        }
    }
}