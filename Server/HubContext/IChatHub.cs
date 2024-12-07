using SignalR.Models;

namespace SignalR.HubContext
{
    public interface IChatHub
    {
        Task SendMessage(Message message);
        Task DeleteMessage(Guid message, string room);
        Task Timeout(string room, string username);
        Task Ban(string room, string username);

    }
}
