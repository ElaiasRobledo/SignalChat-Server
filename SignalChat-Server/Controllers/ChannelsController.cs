using Application.Common.Interfaces.ChannelMembers;
using Application.Common.Interfaces.Channels;
using Application.Common.Interfaces.Utils;
using Application.DTOs.Channels;
using Application.Exceptions.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalChat_Server.Hubs;
using System.Security.Claims;

namespace SignalChat_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            IHttpContextAccessor httpContextAccessor,
            IChannelMember channelMembers,
            ICurrentUserService currentUserService
            )
        {
            _service = service;
            _logger = logger;
            _channelMembers = channelMembers;
            _hub = hub;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllChannelsAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ChannelUpdateDto dto)
        {
            try
            {
                bool ok = await _service.UpdateChannelAsync(id, dto, _currentUserService.UserId);
                return ok ? NoContent() : NotFound();
            }
            catch(MemberNotAuthorizedException ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool ok = await _service.DeleteChannelAsync(id, _currentUserService.UserId);
            return ok ? NoContent() : NotFound();
        }
    }
}