using Microsoft.AspNetCore.SignalR;
using SignalR.HubContext;
using SignalR.Models;
using SignalR.Utility;
using System.Collections.Concurrent;
using System.Timers;
using System.Text.RegularExpressions;
using Timeout = SignalR.Models.Timeout;
using Timer = System.Timers.Timer;
using SignalR.Services;
using System.Reflection;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SignalR.ViewModels;
using System.Net;
using SignalR.Extensions;
using System.Threading;

namespace SignalR
{
    public class ChatService
    {
        #region Fields
        private static Dictionary<string, Room> _rooms;
        private readonly IHubContext<ChatHub> _hub;
        private readonly ConcurrentDictionary<string, Queue<List<Message>>> _messagesQueue;
        private static int _maxOldMsgs = 35;
        private static int _maxShrinkCount = 50;
        public string[] Colors = ["#00ae97", "#ed3b3b", "#edde3b", "#3b97ed", "#ed7c3b", "#c03bed", "#703bed", "#4aba8b", "#8fba4a", "#4a75ba", "#9b47ef", "#68c5d8", "#00ae97", "#ed3b3b", "#edde3b", "#3b97ed", "#ed7c3b", "#c03bed", "#703bed", "#4aba8b", "#8fba4a", "#4a75ba", "#9b47ef", "#68c5d8", "#6cd80d", "#6c5b99", "#5b9996", "#99705b", "#5e995b", "#865b99", "#00f2ff", "#65ff00", "#ffd400", "#ff6100", "#005dff", "#e100ff"];
        #endregion

        #region Constructors
        public ChatService(IHubContext<ChatHub> hub, Dictionary<string, Room> rooms, ConcurrentDictionary<string, Queue<List<Message>>> messagesQueue)
        {
            _hub = hub;
            _rooms = rooms;
            _messagesQueue = messagesQueue;
            //InitializeTimer();
        } 
        #endregion

        #region Services
        public async Task SendMessageAsync(Message message, Guid? replyMessageId)
        {
            var sender = _rooms[message.Sender.ChatRoom!].Connections.Values.FirstOrDefault(s => s.Username == message.Sender.Username);
            if (sender != null)
            {
                if (_rooms[message.Sender.ChatRoom!].Connections.TryGetValue(sender.Id, out User user))
                {
                    Guid Id = Guid.NewGuid();
                    // Setup the Message
                    message.Id = Id;
                    message.Sender = user;
                    message.Date = DateTime.Now;
                    message.State = MessageState.Waiting;
                    if (replyMessageId != null)
                    {
                        var replyMsg = _rooms[message.Sender.ChatRoom!].Messages.FirstOrDefault(s => s.Key == replyMessageId)!.Value;
                        if (replyMsg != null)
                        {
                            message.Reply = replyMsg;
                        }
                    }

                    // Add Message to the room messages
                    _rooms[message.Sender.ChatRoom!].AddMessage(message);


                    //// The Caller
                    //var selfMessage = message;
                    //selfMessage.State = MessageState.Self;
                    //await _hub.Clients.Client(sender.Id).SendAsync("ReceiveMessage", new List<Message> { selfMessage });

                    RegisterMessageQueue(message);

                }
            }
        }

        public async Task<bool> JoinChatAsync(string ConnectionId, User _user, List<Role> roles)
        {
            await CreateRoom(_user.ChatRoom!);
            if (IsRoomExist(_user.ChatRoom!))
            {
                User user = CreateUserIfNotExist(_user, ConnectionId, roles);

                if (_rooms[user.ChatRoom!].Bans.FirstOrDefault(s => s.Username == user.Username) != null)
                {
                    // Update Connection ID 
                    _rooms[user.ChatRoom!].Bans.FirstOrDefault(s => s.Username == user.Username)!.Id = ConnectionId;
                    await _hub.Clients.Client(ConnectionId).SendAsync("Banned");
                    //return true; // Indicate that the connection should be aborted
                }

                if (ConnectionId != null)
                {
                    if (user.Username.ToLower() == user.ChatRoom!.ToLower())
                    {
                        // If the first user in chat will be the broadcaster
                        if (!user.Roles.Contains(Role.Broadcaster))
                            user.Roles.Add(Role.Broadcaster);
                        // If the user roles contain moderator then remove it
                        if (user.Roles.Contains(Role.Moderator))
                            user.Roles.Remove(Role.Moderator);
                        // Set The Broadcaster to channel
                        if (_rooms[user.ChatRoom!].Broadcaster == null)
                            _rooms[user.ChatRoom!].Broadcaster = user;
                    }
                    // Add User If Not Exist
                    if (!IsUserExist(user))
                        _rooms[user.ChatRoom!].Connections[ConnectionId] = user;
                    // Add to Hub Group
                    await _hub.Groups.AddToGroupAsync(ConnectionId, user.ChatRoom!);
                }

                //Add Moderators to the Moderator List
                if (user.Roles.FirstOrDefault(role => role == Role.Moderator) == Role.Moderator &&
                    !_rooms[user.ChatRoom!].Moderators.Contains(user))
                {
                    _rooms[user.ChatRoom!].Moderators.Add(user);
                }


                // Change the old roles user if same username
                var userMsgs = _rooms[user.ChatRoom!].Messages.Where(s => s.Value.Sender.Username == user.Username).ToDictionary();
                foreach (KeyValuePair<Guid, Message> msg in userMsgs)
                {
                    msg.Value.Sender = user;
                }

                //await Clients.Client(Context.ConnectionId)
                //             .SendAsync("WelcomeMessage", message);
                var lastMessages = _rooms[user.ChatRoom!].Messages.TakeLast(_maxOldMsgs).ToDictionary();
                await _hub.Clients.Client(ConnectionId!).SendAsync("WelcomeMessage", user, "Welcome to the chat room!", lastMessages, _rooms[user.ChatRoom!].PinMessage);
                await CurrentUsersAsync(user.ChatRoom!);
                Debug.Log("JOIN_ROOM", ConsoleColor.Green, user.ChatRoom!, user.Username, "has joined");
            }
            return false; // Indicate that the connection should not be aborted
        }

        public async Task SendMessage(Message message)
        {
            var sender = _rooms[message.Sender.ChatRoom!].Connections.Values.FirstOrDefault(s => s.Username == message.Sender.Username);
            if (sender != null)
            {
                if (_rooms[message.Sender.ChatRoom!].Connections.TryGetValue(sender.Id, out User user))
                {
                    // If Messages > 50 DELETE THEM
                    if (_rooms[message.Sender.ChatRoom!].Messages.Count >= 30)
                        _rooms[message.Sender.ChatRoom!].Messages.Clear();


                    Guid Id = Guid.NewGuid();
                    // Setup the Message
                    message.Id = Id;
                    message.Sender = user;
                    message.Date = DateTime.Now;
                    _rooms[message.Sender.ChatRoom!].Messages.Add(Id, message);

                    // The Others in the group
                    await _hub.Clients.GroupExcept(message.Sender.ChatRoom!, sender.Id).SendAsync("ReceiveMessage", message);
                    //Debug.Log("MESSAGE", ConsoleColor.Blue, message.Sender.ChatRoom!, user.Username, $"{message.Text}");

                }
            }
        }

        public async Task DeleteMessageAsync(Guid message, string room)
        {
            if (_rooms[room].Messages.ContainsKey(message))
            {
                var msg = _rooms[room].Messages[message];
                _rooms[room].Messages.Remove(message);
                await _hub.Clients.Group(room).SendAsync("MessageDeleted", message);
            }
        }

        public async Task TimeoutAsync(string room, string username)
        {
            if (IsRoomExist(room))
            {
                var user = _rooms[room].Connections.Values.FirstOrDefault(s => s.Username == username);

                if (user != null && user.Username != _rooms[room].Broadcaster.Username)
                {
                    if (!_rooms[room].Timeouts.ContainsKey(user.Id))
                    {
                        var timeout = new Timeout
                        {
                            User = user,
                            From = DateTime.Now,
                            To = DateTime.Now.AddSeconds(15)
                        };

                        _rooms[room].Timeouts.Add(user.Id, timeout);

                        await CurrentUsersAsync(user.ChatRoom!);

                        foreach (var client in user.Clients)
                        {
                            await _hub.Clients.Client(client).SendAsync("TimeOut", timeout);
                        }

                        // Log the disconnection
                        Debug.Log("USER_TIMEOUT", ConsoleColor.Yellow, user.ChatRoom!, user.Username, "timout for 15 seconds");
                    }
                }
            }
        }

        public async Task BanAsync(string room, string username)
        {
            if (IsRoomExist(room))
            {
                var user = _rooms[room].Connections.Values.FirstOrDefault(s => s.Username == username);
                if (user != null && user.Username != _rooms[room].Broadcaster.Username)
                {
                    if (_rooms[room].Bans.FirstOrDefault(s => s.Username == username) == null)
                    {
                        _rooms[room].Bans.Add(user);
                        foreach (var client in user.Clients)
                        {
                            await _hub.Clients.Client(client).SendAsync("Banned");
                        }
                        //await _hub.Clients.Group(user.ChatRoom!).SendAsync("UserBannedMessage", $"{user.Username} has been banned from Chat");
                        // Remove All Messages 
                        var userMsgs = _rooms[room].Messages.Values.Where(s => s.Sender.Username == user.Username);
                        foreach (var msg in userMsgs)
                        {
                            await DeleteMessageAsync(msg.Id, room);
                        }
                        await CurrentUsersAsync(user.ChatRoom!);
                        // Log Bann Message
                        Debug.Log("USER_BANNED", ConsoleColor.White, user.ChatRoom!, user.Username, "banned from chat and all his messages removed");
                    }
                }
            }
        }

        public async Task UnBanAsync(string room, string username)
        {
            if (IsRoomExist(room))
            {
                var user = _rooms[room].Bans.FirstOrDefault(s => s.Username == username);
                if (user != null)
                {
                    _rooms[room].Bans.Remove(user);
                    foreach (var client in user.Clients)
                    {
                        await _hub.Clients.Client(client).SendAsync("UnBanned");
                    }
                    await CurrentUsersAsync(user.ChatRoom!);
                    // Log Bann Message
                    Debug.Log("USER_UNBANNED", ConsoleColor.White, user.ChatRoom!, user.Username, "unbanned and can Chat now.!");
                }
            }
        }

        public async Task PinAsync(Guid messageId, User sender)
        {

            if (IsRoomExist(sender.ChatRoom!))
            {
                var room = _rooms[sender.ChatRoom!];

                if (room.Connections.Values.FirstOrDefault(s => s.Username == sender.Username) != null && (sender.Roles.Contains(Role.Broadcaster) || sender.Roles.Contains(Role.Moderator)))
                {
                    // Check Message Exist 
                    if (room.Messages.ContainsKey(messageId))
                    {
                        var pin = new PinMessage
                        {
                            Message = room.Messages[messageId],
                            User = sender
                        };
                        room.PinMessage = pin;
                        await _hub.Clients.Group(sender.ChatRoom!).SendAsync("PinMessage", pin);
                        Debug.Log("PIN_MESSAGE", ConsoleColor.White, sender.ChatRoom!, sender.Username, "Has pin message with id = " + messageId.ToString());

                    }
                }
            }
        }

        public async Task UnPinAsync(string roomName)
        {

            if (IsRoomExist(roomName))
            {
                _rooms[roomName].PinMessage = null;

                await _hub.Clients.Group(roomName).SendAsync("UnPinMessage");
                Debug.Log("UNPIN_MESSAGE", ConsoleColor.White, roomName, "Message UnPinned");
            }
        }

        public async Task<IEnumerable<object>> CurrentUsersAsync(string roomName)
        {

            if (IsRoomExist(roomName))
            {
                var Data = _rooms[roomName].Connections.Select(s => new
                {
                    UserInfo = new UserDetailsVM
                    {
                        user = s.Value,
                        IsBanned = IsUserBanned(roomName, s.Value.Username),
                        IsTimeout = IsUserTimedout(roomName, s.Value.Username),
                    },
                    Role = s.Value.Roles.Count > 0 ? (int)s.Value.Roles.Min() : -1
                }).GroupBy(d => d.Role).OrderBy(s => s.Key);

                await _hub.Clients.Group(roomName).SendAsync("CurrentUsers", Data);

                return Data;
            }
            else
                return new List<object>();
        }

        public async Task<ResponseVM> CloseConnectionAsync(string roomName, string clientId)
        {
            if (IsRoomExist(roomName))
            {
                var userId = _rooms[roomName].Connections.FirstOrDefault(s => s.Value.Clients.Contains(clientId));
                if (userId.Key != null)
                {
                    // Remove Join History
                    if (userId.Value.Id != clientId)
                    {
                        await _hub.Clients.Client(clientId).SendAsync("CloseConnection");
                        userId.Value.Clients.Remove(clientId);
                    }
                    else if (userId.Value.Clients.Count > 1)
                    {
                        await _hub.Clients.Client(clientId).SendAsync("CloseConnection");
                        // Change Connection Key to the Next Client                        
                        SwapClientsForUser(roomName, userId.Value, clientId);
                        userId.Value.Clients.Remove(clientId);
                        Debug.Log("CLIENT_LEFT", ConsoleColor.Red, userId.Value.ChatRoom!, userId.Value.Username, $"Client [{userId.Value.Id}] Disconnected");
                    }
                    else
                    {
                        await _hub.Clients.Client(clientId).SendAsync("CloseConnection");
                        _rooms[roomName].Connections.Remove(clientId);
                    }

                    return new ResponseVM
                    {
                        Status = HttpStatusCode.OK,
                        Message = $"Successfully Stopped Client [{clientId}]"
                    };
                }
                else
                {
                    return new ResponseVM
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"The Cient with id [{clientId}] not exist"
                    };
                }
            }
            return new ResponseVM
            {
                Status = HttpStatusCode.NotFound,
                Message = "Room not exist"
            };
        }

        public async Task<ResponseVM> RemoveAsync(string roomName)
        {
            if (IsRoomExist(roomName))
            {
                var clients = _rooms[roomName].Connections.SelectMany(s => s.Value.Clients).ToList();
                foreach (var client in clients)
                {
                    await _hub.Clients.Client(client).SendAsync("CloseConnection");
                    //await _hub.Clients.Client(client).SendAsync("RoomClosed");
                }

                _rooms.Remove(roomName);
                // Send Message To all Group

                return new ResponseVM
                {
                    Status = HttpStatusCode.OK,
                    Message = "Successfully Removed"
                };
            }
            return new ResponseVM
            {
                Status = HttpStatusCode.NotFound,
                Message = "Room not exist"
            };
        }

        public async Task OnDisconnectedAsync(HubCallerContext Context)
        {
            // Find the user and the room they were connected to
            var roomWithUser = _rooms.Values.FirstOrDefault(c => c.Connections != null && c.Connections.FirstOrDefault(s => s.Value.Clients.Contains(Context.ConnectionId)).Key != null);
            if (roomWithUser != null)
            {
                User user = roomWithUser.Connections.FirstOrDefault(s => s.Value.Clients.Contains(Context.ConnectionId)).Value;
                // Check if this the only client for the user 
                if (user.Id != Context.ConnectionId)
                {
                    user.Clients.Remove(Context.ConnectionId);
                }
                else if (user.Clients.Count > 1)
                {
                    SwapClientsForUser(user.ChatRoom!, user, Context.ConnectionId);
                    user.Clients.Remove(Context.ConnectionId);
                    Debug.Log("CLIENT_LEFT", ConsoleColor.Red, user.ChatRoom!, user.Username, $"Client [{Context.ConnectionId}] Disconnected");
                }
                else
                {
                    roomWithUser.Connections.Remove(Context.ConnectionId);

                    // Create the left message
                    string message = $"has left the room.";

                    // Send the message to all clients
                    //await Clients.All.SendAsync("LeftMessage", message);
                    await _hub.Groups.RemoveFromGroupAsync(Context.ConnectionId, user.ChatRoom!);

                    await CurrentUsersAsync(user.ChatRoom!);

                    // Log the disconnection
                    Debug.Log("LEFT_ROOM", ConsoleColor.Red, user.ChatRoom!, user.Username, message);
                }

            }
        } 
        #endregion

        #region Methods
        private User SwapClientsForUser(string roomName,User userId,string clientId)
        {
            // if this client not the only client for the user
            if (userId.Clients.Count > 1)
            {
                var theSecondClient = userId.Clients[1];
                // Update Connection Key and IDs for this user to the new Client ID
                _rooms[roomName].Connections.RenameKey(clientId, theSecondClient);
                userId.Id = theSecondClient;
                // Get user after update
                var userAfterUpdate = _rooms[roomName].Connections.FirstOrDefault(s => s.Value.Clients.Contains(theSecondClient)).Value;
                // If Broadcaster
                if (_rooms[roomName].Broadcaster.Id == clientId)
                    _rooms[roomName].Broadcaster = userAfterUpdate;
                return userAfterUpdate;
            }
            return userId;

        }

        private void RegisterMessageQueue(Message message)
        {
            var room = message.Sender.ChatRoom;
            if (!_messagesQueue.ContainsKey(room))
            {
                _messagesQueue[room] = new Queue<List<Message>>();
            }

            var roomQueue = _messagesQueue[room];

            List<Message> currentListQueue;

            // If the room queue is empty or the last list in the queue is full, create a new list
            if (roomQueue.Count == 0 || roomQueue.Last().Count >= _maxShrinkCount)
            {
                currentListQueue = [message];
                roomQueue.Enqueue(currentListQueue);
            }
            else
            {
                currentListQueue = roomQueue.Last();
                currentListQueue.Add(message);
            }
        }

        private bool IsRoomExist(string room)
        {
            return _rooms.ContainsKey(room);
        }

        private bool IsUserTimedout(string room, string username)
        {
            if (!IsRoomExist(room))
                return false;

            return _rooms[room].Timeouts.Values.FirstOrDefault(s => s.User.Username == username) != null;
        }

        private bool IsUserBanned(string room, string username)
        {
            if (!IsRoomExist(room))
                return false;

            return _rooms[room].Bans.FirstOrDefault(s => s.Username == username) != null;
        }

        private User CreateUserIfNotExist(User user,string ConnectionId ,List<Role> roles)
        {
            var userObj = _rooms[user.ChatRoom!].Connections.FirstOrDefault(s => s.Value.Username == user.Username);

            if (userObj.Key != null)
            {
                if (userObj.Key != null)
                {
                    // Make Id is the new Join Instance
                    userObj.Value.AddJoinHistory(ConnectionId);
                    //_rooms[user.ChatRoom!].Connections.Remove(userObj.Key);
                }
                return userObj.Value;
            }
            else
            {
                user.Color = GetColor(); // Set Color
                user.Roles = roles;
                user.Id = ConnectionId;
                user.AddJoinHistory(ConnectionId);
                return user;
            }
        }

        private bool IsUserExist(User user)
        {
            return _rooms[user.ChatRoom!].Connections.Values.Contains(user);
        }

        private async Task CreateRoom(string name)
        {
            if (!string.IsNullOrEmpty(name) && !_rooms.ContainsKey(name))
            {
                _rooms[name] = new Room
                {
                    Name = name
                };
            }
        }

        private string GetColor()
        {
            var random = new Random();
            int index = random.Next(0, Colors.Length);
            return Colors[index];
        }

        private List<Role> SetUserRole(string[] roles)
        {
            List<Role> userRoles = new List<Role>();
            foreach (var role in roles)
            {
                if (Enum.IsDefined(typeof(Role), role))
                    userRoles.Add((Role)Enum.Parse(typeof(Role), role));
            }
            return userRoles;
        }
        #endregion
    }
}
