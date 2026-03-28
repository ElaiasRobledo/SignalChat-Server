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
using System.Security.Claims;

namespace SignalChat_Server.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/channels")]
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelService _service;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IChannelMember _channelMembers;

        private readonly ILogger<ChannelsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;

        public ChannelsController(IChannelService service,
            ILogger<ChannelsController> logger,
            IHubContext<ChatHub> hub,
            IChannelMember channelMembers,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserService currentUserService
            )
        {
            _service = service;
            _logger = logger;
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
            _channelMembers = channelMembers;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChannelCreateDto dto)
        {
            var result = await _service.CreateChannelAsync(dto, _currentUserService.UserId);
            return Ok(result);
        }

        [HttpPost("join/{channelId}")]
        public async Task<IActionResult> JoinUserToGroup([FromRoute] Guid channelId)
        {
            try
            {
                var connectionId = Request.Headers["X-ConnectionId"].ToString();
                if (string.IsNullOrEmpty(connectionId)) return BadRequest("Missing connectionId");

                var channel = await _service.GetChannelAsync(channelId);
                if (channel is null) { NotFound("Channel has not been found it"); }

                await _channelMembers.JoinToChannel(_currentUserService.UserId, channelId);

                await _hub.Groups.AddToGroupAsync(connectionId, channelId.ToString());
                await _hub.Clients.Groups(channelId.ToString()).SendAsync("ReceiveMessage", $"Welcome {User.Identity?.Name}");

                return Ok();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var channel = await _service.GetChannelAsync(id);
            return channel is null ? NotFound() : Ok(channel);
        }

        [HttpGet("mychannels")]
        public async Task<IActionResult> GetMyChannels()
        {
            
            var channels = await _service.GetMyChannelsAsync(_currentUserService.UserId);
            return Ok(channels);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ChannelUpdateDto dto)
        {
            try
            {
                bool ok = await _service.UpdateChannelAsync(id, dto, _currentUserService.UserId);
                return ok ? NoContent() : NotFound();
            }
            catch (MemberNotAuthorizedException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("search/name")]
        public async Task<ActionResult> GetChannelByName([FromQuery] string name)
        {
            var result = await _service.SearchByNameAsync(name);
            return Ok(result);
        }
         [HttpGet("search/publicId")]
        public async Task<ActionResult> GetChannelByPublicId([FromQuery] int publicId)
        {
            var result = await _service.SearchByPublicIdAsync(publicId);
            return result != null ? Ok(result) : NotFound("Channel not found");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool ok = await _service.DeleteChannelAsync(id, _currentUserService.UserId);
            return ok ? NoContent() : NotFound();
        }
        [HttpDelete("exit/{id}")]
        public async Task<IActionResult> ExitFromAChannel([FromRoute] Guid id)
        {
           
           await _service.ExitFromAGroupAsync(id, _currentUserService.UserId);
           return Ok("You have exited from the channel successfully");

        }
    }
}