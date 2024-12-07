using Swashbuckle.AspNetCore.SwaggerGen;

namespace SignalR.Models
{
    public class Room
    {
        #region Attributes
        public string Name { get; set; }
        public User Broadcaster { get; set; }
        public PinMessage? PinMessage { get; set; }
        public Dictionary<string, User> Connections { get; set; }
        public List<User> Moderators { get; set; }
        public Dictionary<string, Timeout> Timeouts { get; set; }
        public List<User> Bans { get; set; }
        public Dictionary<Guid, Message> Messages { get;}
        public RoomOptions Options { get; set; }
        #endregion

        #region Constructors
        public Room()
        {
            // Initialize Parameters
            Connections = new Dictionary<string, User>();
            Moderators  = new List<User>();
            Timeouts    = new Dictionary<string, Timeout>();
            Bans        = new List<User>();
            Messages    = new Dictionary<Guid, Message>();
            Options     = new RoomOptions
            {
                MessageBufferTime = TimeSpan.FromMilliseconds(200),
                MessagesTimeFactor = 1
            };
        }
        #endregion

        #region Methods
        public void AddMessage(Message message)
        {
            Messages.Add(message.Id,message);
        }
        #endregion
    }
}
