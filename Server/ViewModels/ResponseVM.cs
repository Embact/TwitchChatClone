using SignalR.Models;

namespace SignalR.ViewModels
{
    public class ResponseVM : Response
    {
    }

    public class ResponseVM<T> : Response
        where T : class
    {
        public T Data { get; set; }
    }
}
