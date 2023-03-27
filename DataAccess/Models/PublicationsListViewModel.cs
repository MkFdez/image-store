using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QandAProject.Models;
namespace DataAccess.Models
{
    public class PublicationsListViewModel
    {
        public List<PublicationViewModel> List { get; set; }
        public int Size { get; set; }
        public PublicationsListViewModel(List<PublicationViewModel> _List)
        {
            this.List = _List;
            this.Size = _List.Count;
        }
    }
}