using Application.Common.Interfaces.DbContext;
using Application.Common.Interfaces.Users;
using Microsoft.AspNetCore.SignalR;

namespace SignalChat_Server.Hubs
{
    public class ChatHub : Hub
    {
        //private readonly IUserService _userService;
        //public ChatHub(IUserService userService)
        //{
        //    _userService = userService;
        //}

        public async Task SendMessageToAllClients(string user, string message)
        => await Clients.All.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToSpecificClient(string targetUserId, string message)
        {
           // var targetUser = await _userService.
            await Clients.User(targetUserId).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }
    }
}
