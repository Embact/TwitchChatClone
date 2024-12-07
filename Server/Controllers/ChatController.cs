using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using SignalR.HubContext;
using SignalR.Managers;
using SignalR.Models;
using SignalR.Services;
using SignalR.ViewModels;
using System.Collections.Generic;
using System.Net;

namespace SignalR.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatController : Controller
    {
        private readonly Dictionary<string, Room> _rooms;
        private readonly IHubContext<ChatHub, IChatHub> _hub;
        private readonly ChatService _chatService;

        public ChatController(Dictionary<string, Room> rooms, IHubContext<ChatHub, IChatHub> hub, ChatService chatService)
        {
            _rooms = rooms;
            _hub = hub;
            _chatService = chatService;
        }

        [HttpGet]
        [Route("allrooms")]
        public IActionResult GetAllRooms()
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                return Ok(_manager.GetAllRooms());
            }
        }

        [HttpGet]
        [Route("roomsNames")]
        public IActionResult GetAllRoomsNames()
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                return Ok(_manager.GetAllRoomsNames());
            }
        }

        [HttpGet]
        [Route("{room}")]
        public IActionResult GetRoom(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.GetRoom(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }

        }

        [HttpGet]
        [Route("{room}/connections")]
        public IActionResult GetRoomConnection(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.GetRoomConnection(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpGet]
        [Route("{room}/timeouts")]
        public IActionResult GetRoomTimeouts(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.GetRoomTimeouts(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpGet]
        [Route("{room}/bans")]
        public IActionResult GetRoomBans(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.GetRoomBans(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpGet]
        [Route("{room}/messages")]
        public IActionResult GetRoomMessages(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.GetRoomMessages(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpGet]
        [Route("{room}/currentUsers")]
        public IActionResult CurrentUsers(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.CurrentUsers(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpDelete]
        [Route("{room}/{name}/closeConnection")]
        public async Task<IActionResult> CloseConnection(string room,string name)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = await _manager.CloseConnection(room, name);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpDelete]
        [Route("{room}/{clientId}/closeClientConnection")]
        public async Task<IActionResult> CloseClientConnection(string room,string clientId)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = await _manager.CloseClientConnection(room, clientId);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpDelete]
        [Route("{room}/remove")]
        public async Task<IActionResult> Remove(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = await _manager.Remove(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpDelete]
        [Route("removeAll")]
        public async Task<IActionResult> RemoveAll()
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                return Ok(_manager.RemoveAll());
            }
        }

        [HttpPost]
        [Route("{room}/ban")]
        public IActionResult Ban([BindRequired] string room, [BindRequired] string user)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.Ban(room, user);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpPost]
        [Route("{room}/unban")]
        public IActionResult Unban([BindRequired] string room, [BindRequired] string user)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                return Ok(_manager.Unban(room, user));
            }
        }

        [HttpPost]
        [Route("{room}/timeout")]
        public IActionResult Timeout([BindRequired] string room, [BindRequired] string user)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.Timeout(room, user);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpPost]
        [Route("{room}/sendMessage")]
        public IActionResult SendMessage([BindRequired] string Message, [BindRequired] string room, [BindRequired] string user,Guid replyMsgId)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.SendMessage(Message,room, user, replyMsgId);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpPost]
        [Route("{room}/pinMessage")]
        public IActionResult PinMessage([BindRequired] Guid MessageId, [BindRequired] string room,[BindRequired] string user)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.PinMessage(MessageId, room, user);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpPost]
        [Route("{room}/unPinMessage")]
        public IActionResult UnPinMessage(string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.UnPinMessage(room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpPost]
        [Route("{room}/deleteMessage")]
        public IActionResult DeleteMessage([BindRequired] Guid MessageId, [BindRequired] string room)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = _manager.DeleteMessage(MessageId, room);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }

        [HttpGet]
        [Route("{room}/toggleAutoChat")]
        public async Task<IActionResult> ToggleAutoChat([BindRequired] string room, [BindRequired] int interval, [BindRequired] bool isParallel,int parallelLength)
        {
            using (var _manager = new ChatManager(_rooms, _chatService))
            {
                var roomResult = await _manager.ToggleAutoChat(room, interval, isParallel, parallelLength);
                return roomResult.Status == HttpStatusCode.OK ? Ok(roomResult) : NotFound(roomResult);
            }
        }
    }
}
