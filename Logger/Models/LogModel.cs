using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger.Models
{
    public class LogModel
    {
        public string Message { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set;}
        public LogType type { get; set; }
    }
}
