using System;

namespace ChatServicesII.Models
{

    public class ChatViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastMessage { get; set; }
        public int UnreadedMessages { get; set; }
    }
}
