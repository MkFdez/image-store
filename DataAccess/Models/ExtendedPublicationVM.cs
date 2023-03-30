using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ExtendedPublicationVM
    {
        public ExtendedPublicationVM(PublicationViewModel publication, string guid, string filename)
        {
            Publication = publication;
            Guid = guid;
            Filename = filename;
        }

        public PublicationViewModel Publication { get; set; }
        public string Guid { get; set; }
        public string Filename { get; set; }
    }
}
