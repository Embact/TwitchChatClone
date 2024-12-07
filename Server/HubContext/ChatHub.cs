using Microsoft.AspNetCore.SignalR;
using SignalR.Models;
using SignalR.Services;
using SignalR.Utility;
using Timeout = SignalR.Models.Timeout;

namespace SignalR.HubContext
{

    public sealed class ChatHub : Hub<IChatHub>
    {
        private ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task JoinChat(User user, List<Role> roles)
        {
            bool shouldAbort = await _chatService.JoinChatAsync(Context.ConnectionId, user, roles);

            if (shouldAbort)
            {
                Context.Abort();
            }
        }

        public async Task SendMessage(Message message,Guid? replyMessageId)
        {
            _chatService.SendMessageAsync(message, replyMessageId);
        }

        public async Task DeleteMessage(Guid message, string room)
        {
            await _chatService.DeleteMessageAsync(message, room);
        }

        public async Task Timeout(string room, string username)
        {
           await _chatService.TimeoutAsync(room, username);
        }

        public async Task Ban(string room, string username)
        {
            await _chatService.BanAsync(room, username);
        }

        public async Task Pin(Guid messageId,User sender)
        {
            await _chatService.PinAsync(messageId, sender);
        }

        public async Task UnPin(string room)
        {
            await _chatService.UnPinAsync(room);
        }

        public async Task CurrentUsers(string room)
        {
            await _chatService.CurrentUsersAsync(room);
        }

        public async Task CloseConnection(string room,string connectionId)
        {
            await _chatService.CloseConnectionAsync(room,connectionId);
        }

        public async Task Remove(string room)
        {
            await _chatService.RemoveAsync(room);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _chatService.OnDisconnectedAsync(Context);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
