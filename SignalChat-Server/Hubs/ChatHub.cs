using Application.Common.Interfaces.ChannelMembers;
using Application.Common.Interfaces.Users;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SignalChat_Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChannelMember _channelMember;
        public ChatHub(IChannelMember channelMember)
        {
            _channelMember = channelMember;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var channels = await _channelMember.GetChannelsForUserAsync(Guid.Parse(userId));
            foreach (var channel in channels)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, channel.ChannelId.ToString());
            }
            await base.OnConnectedAsync();
            Console.WriteLine($"ConnectionId: {Context.ConnectionId}");

        }

        public async Task SendMessageToAllClients(string user, string message)
        => await Clients.All.SendAsync("ReceiveMessage", user, message);
        public async Task SendMessageToSpecificClient(string targetUserId, string message)
        => await Clients.User(targetUserId).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);

        public async Task SendMessageToGroup(Guid channelId,string message)
        => await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);

        public async Task WelcomeMessageToGroup(string connectionId, Guid channelId)
        {

            await Groups.AddToGroupAsync(connectionId, channelId.ToString());
            await Clients.Group(channelId.ToString()).SendAsync("ReceiveMessage", $"Welcome {Context.User.Identity.Name}");

        }
        public async Task<string> GetConnectionId()
        {
            return Context.ConnectionId;
        }



    }

}

