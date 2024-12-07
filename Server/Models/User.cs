using SignalR.Utility;

namespace SignalR.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string? ChatRoom { get; set; }
        public string? Color { get; set; }
        public List<Role> Roles { get; set; }
        public List<string> Clients { get; set; }

        public User()
        {
            Clients = new List<string>();
        }

        public void AddJoinHistory(string id)
        {
            if (!Clients.Contains(id))
                Clients.Add(id);
        }
    }
}
