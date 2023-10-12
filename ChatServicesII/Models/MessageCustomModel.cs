using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServicesII.Models
{
    public class MessageCustomModel
    {
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string Sender { get; set; }
        public string FilePath { get; set; }
        public MessageCustomModel(string content, DateTime date, string sender, string path)
        {
            Content = content;
            Date = date;
            Sender = sender;
            FilePath = path;
        }
        public MessageCustomModel() { }
    }
}
