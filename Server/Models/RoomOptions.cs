namespace SignalR.Models
{
    public class RoomOptions
    {
        public TimeSpan MessageBufferTime { get; set; }
        public int MessagesTimeFactor { get; set; }
    }
}
