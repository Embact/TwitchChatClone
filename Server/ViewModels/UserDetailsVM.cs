using SignalR.Models;

namespace SignalR.ViewModels
{
    public class UserDetailsVM
    {
        public bool IsBanned { get; set; }
        public bool IsTimeout { get; set; }
        public User? user { get; set; }
    }
}
