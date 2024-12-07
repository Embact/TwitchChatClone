using System.Net;

namespace SignalR.Models
{
    public abstract class Response
    {
        public HttpStatusCode Status { get; set; }
        public string? Message { get; set; }
    }
}
