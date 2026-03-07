using Application.Common.Interfaces.Channels;
using Application.Common.Interfaces.Utils;
using Application.DTOs.Channels;
using Application.Exceptions.ChannelMembers;
using Application.Exceptions.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalChat_Server.Hubs;

namespace SignalChat_Server.Controllers
{
    [ApiController]
    [Route("api/channels/{channelId}/requests")]
    public class ChannelJoinRequestsController : ControllerBase
    {
        private readonly IChannelJoinRequest _channelJoinRequests;
        private readonly IChannelMembership _channelMembership;
        private readonly IChannelModeration _channelModeration;
        private readonly IHubContext<ChatHub> _hub;
        private readonly ICurrentUserService _currentUser;

        private readonly ILogger<ChannelJoinRequestsController> _logger;

        public ChannelJoinRequestsController(IChannelJoinRequest channelJoinRequests,
            IChannelMembership channelMembership,
            IChannelModeration channelModeration,
            ILogger<ChannelJoinRequestsController> logger,
            ICurrentUserService currentUserService,
            IHubContext<ChatHub> hub)
        {
            _channelJoinRequests = channelJoinRequests;
            _channelMembership = channelMembership;
            _channelModeration = channelModeration;
            _hub = hub;
            _logger = logger;
            _currentUser = currentUserService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetIncomingRequests([FromRoute] Guid channelId)
        {
            try
            {
                var result = await _channelJoinRequests.IncomingRequestsAsync(channelId, _currentUser.UserId);
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
        [HttpPost("join")]
        public async Task<IActionResult> SendJoinRequest(CreateJoinRequestDto request, [FromRoute] Guid channelId)
        {
            try
            {
                await _channelJoinRequests.CreateJoinRequestAsync(
               _currentUser.UserId, channelId, request.Reason);
            }
            catch (ChannelNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (SentRequestToJoinToChannelException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(OwnerSendJoinRequestException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created();
        }

        //Add catching errors and expection and return better responses.
 
        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> ApproveRequest ([FromRoute] Guid channelId, [FromRoute] Guid userId)
        {
            await _channelJoinRequests.ApproveAsync(channelId, userId, _currentUser.UserId);
            return Ok("User approved successfully");

        }
        [HttpPut("reject/{userId}")]
        public async Task<IActionResult> RejectRequest ([FromRoute] Guid channelId, [FromRoute] Guid userId)
        {
            await _channelJoinRequests.RejectAsync(channelId, userId, _currentUser.UserId);
            return Ok("User rejected successfully");

        }

    }
}
