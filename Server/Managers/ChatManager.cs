using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using SignalR.HubContext;
using SignalR.Models;
using SignalR.Services;
using SignalR.Utility;
using SignalR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using Timeout = SignalR.Models.Timeout;

namespace SignalR.Managers
{
    public class ChatManager : IDisposable
    {
        private static bool IsAutoChat = false;
        private readonly Dictionary<string, Room> _rooms;
        private readonly ChatService _chatService;

        public ChatManager(Dictionary<string, Room> rooms, ChatService chatService)
        {
            _rooms = rooms;
            _chatService = chatService;
        }

        public Response GetAllRooms()
        {
            return new ResponseVM<Dictionary<string,Room>>
            {
                Data = _rooms,
                Message = "Successfully Get All Rooms",
                Status = HttpStatusCode.OK
            };
        }    
        
        public Response GetAllRoomsNames()
        {
            var roomsNames = _rooms.Values.Select(s => s.Name).ToList();
            return new ResponseVM<List<string>>
            {
                Data = roomsNames,
                Message = "Successfully Get All Rooms Names",
                Status = HttpStatusCode.OK
            };
        }

        public Response GetRoom(string roomName)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            return new ResponseVM<Room>
            {
                Data = _rooms[roomName],
                Message = "Successfully Get All Rooms Names",
                Status = HttpStatusCode.OK
            };
        }

        public Response GetRoomConnection(string roomName)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            return new ResponseVM<Dictionary<string, User>>
            {
                Data = _rooms[roomName].Connections,
                Message = "Successfully Get Connections",
                Status = HttpStatusCode.OK
            };
        }

        public Response GetRoomTimeouts(string roomName)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            return new ResponseVM<Dictionary<string, Timeout>>
            {
                Data = _rooms[roomName].Timeouts,
                Message = "Successfully Get Timeouts",
                Status = HttpStatusCode.OK
            };
        }

        public Response GetRoomBans(string roomName)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            return new ResponseVM<List<User>>
            {
                Data = _rooms[roomName].Bans,
                Message = "Successfully Get Bans",
                Status = HttpStatusCode.OK
            };
        }

        public Response GetRoomMessages(string roomName)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            return new ResponseVM<IEnumerable<object>>
            {
                Data = _rooms[roomName].Messages.Select(s => new
                {
                    s.Value.Id,
                    s.Value.Text,
                    s.Value.Date
                }),
                Message = "Successfully Get Messages",
                Status = HttpStatusCode.OK
            };
        }

        public Response CurrentUsers(string roomName)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            var data = _chatService.CurrentUsersAsync(roomName).Result;
            return new ResponseVM<IEnumerable<object>>
            {
                Data = data,
                Message = "Successfully Get Messages",
                Status = HttpStatusCode.OK
            };
        }

        public async Task<Response> CloseConnection(string roomName, string name)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            var clientId = _rooms[roomName].Connections.FirstOrDefault(s => s.Value.Username == name).Key;

            var close = await _chatService.CloseConnectionAsync(roomName, clientId);

            return new ResponseVM
            {
                Status = close.Status,
                Message = close.Message
            };
        }

        public async Task<Response> CloseClientConnection(string roomName,string clientId)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            var close = await _chatService.CloseConnectionAsync(roomName, clientId);

            return new ResponseVM
            {
                Status = close.Status,
                Message = close.Message
            };
        }

        public async Task<Response> Remove(string roomName)
        {
            if (!IsRoomExist(roomName))
                return NotFoundRoom();

            await _chatService.RemoveAsync(roomName);

            return new ResponseVM
            {
                Message = "Successfully Remove Room",
                Status = HttpStatusCode.OK
            };
        }


        public async Task<Response> RemoveAll()
        {
            foreach (var room in _rooms)
            {
                await _chatService.RemoveAsync(room.Key);
            }

            return new ResponseVM
            {
                Message = "Successfully Remove All Rooms",
                Status = HttpStatusCode.OK
            };
        }

        public Response Ban(string room,string user)
        {
            var userInfo = UserInfo(room, user);
            if (userInfo.Status != HttpStatusCode.OK)
                return userInfo;

            // Do Something If User & Room Are found
            var userInfoResult = ((ResponseVM<UserDetailsVM>)userInfo).Data;
            _chatService.BanAsync(room, user).Wait();
            return new ResponseVM
            {
                Status = HttpStatusCode.OK,
                Message = $"{userInfoResult.user!.Username} Successfully Banned"
            };
        }

        public Response Unban(string room,string user)
        {

            if (!IsRoomExist(room))
                return new ResponseVM
                {
                    Status = HttpStatusCode.NotFound,
                    Message = $"Room {room} Not found"
                };

            if (!IsUserBanned(room, user))
                return new ResponseVM
                {
                    Status = HttpStatusCode.NotFound,
                    Message = $"User {user} Not banned"
                };


            _chatService.UnBanAsync(room, user).Wait();
            return new ResponseVM
            {
                Status = HttpStatusCode.OK,
                Message = $"{user} Successfully Unbanned"
            };
        }

        public Response Timeout(string room,string user)
        {

            var userInfo = UserInfo(room, user);
            if (userInfo.Status != HttpStatusCode.OK)
                return userInfo;

            // Do Something If User & Room Are found
            var userInfoResult = ((ResponseVM<UserDetailsVM>)userInfo).Data;
            _chatService.TimeoutAsync(room, user).Wait();
            return new ResponseVM
            {
                Status = HttpStatusCode.OK,
                Message = $"{userInfoResult.user!.Username} Successfully timmed out"
            };
        }

        public Response SendMessage(string Message,string room, string user, Guid replyMsgId)
        {
            if (!IsRoomExist(room))
                return NotFoundRoom();

            if (IsUserExist(room, user))
            {
                if (IsUserTimedout(room, user))
                {
                    return new ResponseVM
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"{user} Timmed Out ,Cant send message"
                    };
                }

                if (IsUserBanned(room, user))
                {
                    return new ResponseVM
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"{user} Banned , Cant send message"
                    };
                }

                var userSender = _rooms[room].Connections.Values.FirstOrDefault(s => s.Username == user);

                Message msg = new Message
                {
                    Date = DateTime.Now,
                    Sender = userSender,
                    Id = Guid.NewGuid(),
                    Text = Message
                };
                _chatService.SendMessageAsync(msg, replyMsgId).Wait();
                return new ResponseVM<Message>
                {
                    Status = HttpStatusCode.OK,
                    Message = $"{userSender.Username} Message Sended",
                    Data = msg
                };
            }
            return new ResponseVM
            {
                Status = HttpStatusCode.NotFound,
                Message = $"{user} Not found"
            };
        }

        public Response PinMessage(Guid MessageId, string room, string user)
        {
            if (!IsRoomExist(room))
                return NotFoundRoom();

            if (IsUserExist(room, user))
            { 
                var userSender = _rooms[room].Connections.Values.FirstOrDefault(s => s.Username == user);
                if (userSender!.Roles.Contains(Role.Broadcaster) || userSender.Roles.Contains(Role.Moderator))
                {
                    _chatService.PinAsync(MessageId, userSender!).Wait();
                    return new ResponseVM
                    {
                        Status = HttpStatusCode.OK,
                        Message = $"{MessageId} Message Pinned",
                    };
                } 
                else
                {
                    return new ResponseVM
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = $"{user} Not a moderator"
                    };
                }
               
            }
            return new ResponseVM
            {
                Status = HttpStatusCode.NotFound,
                Message = $"{user} Not found"
            };
        }

        public Response UnPinMessage(string room)
        {
            if (!IsRoomExist(room))
                return NotFoundRoom();


            if (_rooms[room].PinMessage != null)
            {
                _chatService.UnPinAsync(room).Wait();
                return new ResponseVM
                {
                    Status = HttpStatusCode.OK,
                    Message = $"Message UnPinned",
                };

            }
            return new ResponseVM
            {
                Status = HttpStatusCode.NotFound,
                Message = $"Not found Pin Message"
            };
        }

        public Response DeleteMessage(Guid MessageId, string room)
        {
            if (!IsRoomExist(room))
                return NotFoundRoom();
    
            _chatService.DeleteMessageAsync(MessageId, room).Wait();

            return new ResponseVM
            {
                Status = HttpStatusCode.OK,
                Message = $"Message Successfully Deleted",
            };
        }
        
        public async Task<Response> ToggleAutoChat(string room, int interval,bool isParallel, int parallelLength)
        {
            if (!IsRoomExist(room))
                return NotFoundRoom();
            IsAutoChat = !IsAutoChat;  // Toggle the auto chat state

            if (IsAutoChat)
            {
                Debug.Log("AUTO_CHAT",ConsoleColor.White,$"AutoChat [Started] , Parallel: {isParallel} , Messages [{parallelLength}] Per {interval}ms");
                // Start Logic
                // Read From messages.json
                string fileName = "messages.json";
                string fileContent = string.Empty;

                try
                {
                    fileContent = await File.ReadAllTextAsync(fileName);
                }
                catch (Exception ex)
                {
                    return new ResponseVM
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = $"Error reading messages.json: {ex.Message}"
                    };
                }

                // Get Messages
                List<string> messages;

                try
                {
                    messages = JsonSerializer.Deserialize<List<string>>(fileContent)!;
                }
                catch (Exception ex)
                {
                    return new ResponseVM
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = $"Error deserializing messages.json: {ex.Message}"
                    };
                }

                // Get Connections in chat room
                var users = _rooms[room].Connections.Where(s => !_rooms[room].Bans.Contains(s.Value) && !_rooms[room].Timeouts.Values.Any(d => d.User.Username == s.Value.Username)).ToDictionary().Values.ToList();
                // Random Instance 
                Random random = new Random();


                // Create the Message
                _ = Task.Run(async () =>
                {
                    while (IsAutoChat)
                    {
                        if (isParallel)
                        {
                            await GenerateParallelRandomMessages(messages, users, random, parallelLength);
                        } 
                        else
                        {
                            Message generatedMessage = GenerateRandomMessage(messages, users, random);
                            await _chatService.SendMessageAsync(generatedMessage,null);
                        }

                        await Task.Delay(interval);
                    }
                });

                return new ResponseVM<List<string>>
                {
                    Status = HttpStatusCode.OK,
                    Message = $"Auto Chat Started...",
                    Data = messages
                };
            } 
            else
            {
                Debug.Log("AUTO_CHAT", ConsoleColor.White, $"AutoChat [Stopped]");
                IsAutoChat = false;
                return new ResponseVM
                {
                    Status = HttpStatusCode.OK,
                    Message = $"Auto Chat Stopped.",
                };
            }
        }

        #region Helpers
        private async Task GenerateParallelRandomMessages(List<string> messages, List<User> users, Random random,int parallelLength = 10)
        {
            var msgList = new List<Message>();

            for (int i = 0; i < parallelLength; i++)
            {
                msgList.Add(GenerateRandomMessage(messages, users, random));
            }

            var tasks = new List<Task>();

            foreach (var message in msgList)
            {
                tasks.Add(SendParallelMessages(message));
            }
            await Task.WhenAll(tasks);
        }
        private async Task SendParallelMessages(Message message)
        {
            await _chatService.SendMessageAsync(message,null);
        }
        private Message GenerateRandomMessage(List<string> messages, List<User> users,Random random)
        {
            // Random Message 
            string randomMessage = messages[random.Next(0, messages.Count)];
            User randomUser = users[random.Next(0, users.Count)];

            return new Message
            {
                Text = randomMessage,
                Sender = randomUser,
                Date = DateTime.Now,
                Id = Guid.NewGuid(),
                State = MessageState.Waiting
            };
        }
        private Response NotFoundRoom()
        {
            return new ResponseVM
            {
                Message = $"Not Found Room",
                Status = HttpStatusCode.NotFound
            };
        }

        private bool IsRoomExist(string room)
        {
            return _rooms.ContainsKey(room);
        }

        private bool IsUserExist(string room, string username)
        {
            if (!IsRoomExist(room))
                return false;

            return _rooms[room].Connections.Values.FirstOrDefault(s => s.Username == username) != null;
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

        private Response UserInfo(string room, string username)
        {
            if (!IsRoomExist(room))
                return new ResponseVM
                {
                    Status = HttpStatusCode.NotFound,
                    Message = $"Room {room} Not found"
                };

            if (!IsUserExist(room, username))
                return new ResponseVM
                {
                    Status = HttpStatusCode.NotFound,
                    Message = $"User {username} Not found"
                };

            var userSender = _rooms[room].Connections.Values.FirstOrDefault(s => s.Username == username);
            return new ResponseVM<UserDetailsVM>
            {
                Status = HttpStatusCode.OK,
                Message = $"Succesffully Found Room",
                Data = new UserDetailsVM
                {
                    IsBanned = IsUserBanned(room, username),
                    IsTimeout = IsUserTimedout(room, username),
                    user = userSender!
                }
            };
        }
        #endregion

        public void Dispose()
        {
           
        }
    }
}
