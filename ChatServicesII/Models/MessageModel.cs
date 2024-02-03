namespace ChatServicesII.Models
{
    public class MessageModel
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public string FilePath { get; set; }
        public MessageModel(string message, string sender ) {
            Message = message;
            Sender = sender;
        }
        public MessageModel(string message, string sender,string path)
        {
            Message = message;
            Sender = sender;
            FilePath = path;
        }
        public MessageModel() { }
    }
}
