using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TemporalViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public List<SimplePublicationViewModel> Publications { get; set; }
    }
}
