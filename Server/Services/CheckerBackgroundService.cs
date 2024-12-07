
using Microsoft.AspNetCore.SignalR;
using SignalR.HubContext;
using SignalR.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Timeout = SignalR.Models.Timeout;

namespace SignalR.Services
{
    public class CheckerBackgroundService : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly Dictionary<string, Room> _rooms;
        private readonly ConcurrentDictionary<string, Queue<List<Message>>> _messageQueue;

        public CheckerBackgroundService(IHubContext<ChatHub> hubContext, Dictionary<string, Room> rooms,
             ConcurrentDictionary<string, Queue<List<Message>>> messageQueue)
        {
            _hubContext = hubContext;
            _rooms = rooms;
            _messageQueue = messageQueue;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {                

                foreach (var room in _rooms)
                {
                    var now = DateTime.Now;
                    var expiredTimeouts = room.Value.Timeouts.Where(s => s.Value.To <= now).ToDictionary();

                    foreach (var timeout in expiredTimeouts)
                    {
                        room.Value.Timeouts.Remove(timeout.Key);
                        var userClients = room.Value.Connections.FirstOrDefault(s => s.Value.Username == timeout.Value.User.Username).Value.Clients;
                        foreach (var client in userClients)
                        {
                            await _hubContext.Clients.Client(client).SendAsync("TimeOutFinished", timeout);
                        }
                    }


                    if (room.Value.Connections.Count == 0)
                    {
                        // Remove Room
                        _rooms.Remove(room.Key);
                    }
                }
                await Task.Delay(1000, stoppingToken); // Check every second
                Debug.Status(_rooms, _messageQueue);
            }
        }
    }
}
