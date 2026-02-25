using Application.Common.Interfaces.Channels;
using Application.Common.Interfaces.Utils;
using Application.DTOs.Channels;
using Application.Exceptions.ChannelMembers;
using Application.Exceptions.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SignalChat_Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]

    public class ChannelMembersController : ControllerBase
    {
        private readonly IChannelJoinRequest _channelJoinRequests;
        private readonly IChannelMembership _channelMembership;
        private readonly IChannelModeration _channelModeration;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<ChannelMembersController> _logger;

        public ChannelMembersController(IChannelJoinRequest channelJoinRequests, 
            IChannelMembership channelMembership, 
            IChannelModeration channelModeration, 
            ILogger<ChannelMembersController> logger,
            ICurrentUserService currentUserService)
        {
            _channelJoinRequests = channelJoinRequests;
            _channelMembership = channelMembership;
            _channelModeration = channelModeration;
            _logger = logger;
            _currentUser = currentUserService;
        }

        [HttpGet("pendingrequests/{id}")]
        public async Task<IActionResult> GetIncomingRequests(Guid id)
        {
            try
            {
                var result = await _channelJoinRequests.IncomingRequestsAsync(id, _currentUser.UserId);
                return Ok(result);
            }
            catch (MemberNotAuthorizedException ex)
            { return Unauthorized(ex.Message); }
            catch (ChannelNotFoundException ex)
            { return NotFound(ex.Message); }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("joinrequest/{id}")]
        public async Task<IActionResult> SendJoinRequest(CreateJoinRequestDto request, [FromRoute] Guid id)
        {
            try
            {
                await _channelJoinRequests.SendRequestToJoinToPrivateChannel(
               _currentUser.UserId, id, request.Reason);
            }
            catch (ChannelNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (SentRequestToJoinToChannelException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created();
        }
    }
}
