using System;

namespace ChatServices.Models
{

    public class ChatViewModel
    {
        public string Name { get; set; }
        public DateTime LastMessage { get; set; }
        public int UnreadedMessages { get; set; }
        public ChatViewModel(string name, DateTime last, int unread)
        {
            Name = name;
            LastMessage = last;
            UnreadedMessages = unread;
        }
    }
}
