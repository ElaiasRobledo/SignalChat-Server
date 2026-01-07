using Application.Common.Interfaces.Users;
using Microsoft.AspNetCore.SignalR;

namespace SignalChat_Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToAllClients(string user, string message)
        => await Clients.All.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToSpecificClient(string targetUserId, string message)
        {
            await Clients.User(targetUserId).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }
     
    }
}
