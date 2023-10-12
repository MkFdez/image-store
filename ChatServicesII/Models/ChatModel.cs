using System;
using System.Collections.Generic;


namespace ChatServicesII.Models
{

    public class ChatModel
    {

        public string Name { get; set; }
        public List<MessageModel> Messages { get; set; }

        public DateTime LastMessage { get; set; }
        public int UnreadedMessages { get; set; }
        public ChatModel(string name)
        {
            Name = name;
            Messages = new List<MessageModel>();
            UnreadedMessages = 0;
        }


    }
}