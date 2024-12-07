
using Microsoft.AspNetCore.SignalR;
using SignalR.HubContext;
using SignalR.Models;
using SignalR.Utility;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SignalR.Services
{
    public class MessageProcessingService : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly Dictionary<string, Room> _rooms;
        private readonly ConcurrentDictionary<string, Queue<List<Message>>> _messageQueue;
        private static int LastWaitingMessages = 0;

        public MessageProcessingService(IHubContext<ChatHub> hubContext, Dictionary<string, Room> rooms, ConcurrentDictionary<string, Queue<List<Message>>> messageQueue)
        {
            _hubContext = hubContext;
            _rooms = rooms;
            _messageQueue = messageQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessMessagesAsync();

                await Task.Delay(100, stoppingToken); ; // Delay between processing 
            }
        }

        private TimeSpan GetDelayTime(RoomOptions options)
        {
            // Group messages by room
            if (_rooms.Count > 0)
            {
                var MS = _rooms.First().Value.Options.MessageBufferTime.TotalMilliseconds *
                    _rooms.First().Value.Options.MessagesTimeFactor;
                return TimeSpan.FromMilliseconds(MS < 0 ? 0 : MS > 1000 ? 700 : MS);
            }
            else
                return TimeSpan.FromMilliseconds(1000);
        }

        //public void EnqueueMessage(Message message)
        //{
        //    _messageQueue.Enqueue(message);
        //}

        private async Task ProcessMessagesAsync()
        {

            foreach (var roomQueue in _messageQueue)
            {
                var room = roomQueue.Key;
                var queue = roomQueue.Value;

                while (queue.TryDequeue(out var messageList))
                {
                    if (messageList.Any())
                    {
                        var waitingMessages = roomQueue.Value.Sum(s => s.Count);
                        if (waitingMessages > LastWaitingMessages || waitingMessages == 0)
                        {
                            HandleRoomDelay(room, waitingMessages);
                        }
                        // Send batched messages to the group
                        await _hubContext.Clients.Group(room).SendAsync("ReceiveMessage", messageList);
                        Debug.Log("MSGs_BUFFERED",ConsoleColor.DarkYellow, room+"`s ROOM", messageList.Count.ToString() + $" Factor {_rooms[room].Options.MessagesTimeFactor} Time {_rooms[room].Options.MessageBufferTime.TotalMilliseconds} Buffer IN {GetDelayTime(_rooms[room].Options).TotalMilliseconds}ms");
                        await Task.Delay(GetDelayTime(_rooms[room].Options));
                    }
                }
            }
        }

        private void HandleRoomDelay(string room, int waitingMessages)
        {
            // Calculate the change in waiting messages            
            int changeInMessages = waitingMessages - LastWaitingMessages;

            if (waitingMessages <= 2)
                _rooms[room].Options.MessagesTimeFactor = 0;
            else if (waitingMessages > 2 && waitingMessages <= 100)
                _rooms[room].Options.MessagesTimeFactor = 1;
            else if (waitingMessages > 100 && waitingMessages <= 600)
                _rooms[room].Options.MessagesTimeFactor = 2;
            //else if (waitingMessages > 600 && waitingMessages <= 1000)
            //    _rooms[room].Options.MessagesTimeFactor = 3;
            //else if (changeInMessages > 200 && changeInMessages <= 600)
            //    _rooms[room].Options.MessagesTimeFactor = 4;
            //else if (changeInMessages > 600 && changeInMessages <= 1000)
            //    _rooms[room].Options.MessagesTimeFactor = 5;
            //else
            //    _rooms[room].Options.MessagesTimeFactor = 4;

            LastWaitingMessages = waitingMessages;
        }
    }
}
