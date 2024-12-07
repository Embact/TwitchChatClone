using SignalR.Utility;

namespace SignalR.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public User Sender { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public Message? Reply { get; set; }
        public MessageState? State { get; set; }
    }
}
