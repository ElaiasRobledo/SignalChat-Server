using Application.Common.Interfaces.ChannelMembers;
using Application.Common.Interfaces.Channels;
using Application.Common.Interfaces.Utils;
using Application.DTOs.Channels;
using Application.Exceptions.ChannelMembers;
using Application.Exceptions.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalChat_Server.Hubs;

namespace SignalChat_Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/channels/{channelId}/members")]

    public class ChannelMembersController : ControllerBase
    {
        private readonly IChannelMember _channelMembers;
        private readonly ILogger<ChannelMembersController> _logger;
        
        public ChannelMembersController(IChannelMember channelMember, ILogger<ChannelMembersController> logger)
        {
            _channelMembers = channelMember;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetMembers([FromRoute] Guid channelId)
        {
            var response = await _channelMembers.GetMembersAsync(channelId);
            return Ok(response);
        }
    }
}
