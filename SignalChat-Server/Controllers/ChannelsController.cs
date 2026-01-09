using Application.Common.Interfaces.ChannelMembers;
using Application.Common.Interfaces.Channels;
using Application.DTOs.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ChatHub _hub;
        private readonly IChannelMember _channelMembers;
        private readonly ILogger<ChannelsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChannelsController(IChannelService service,
            ILogger<ChannelsController> logger,
            IHttpContextAccessor httpContextAccessor,
            IChannelMember channelMembers
            )
        {
            _service = service;
            _logger = logger;
            _channelMembers = channelMembers;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChannelCreateDto dto)
        {
            var result = await _service.CreateChannelAsync(dto);
            return Ok(result);
        }
        [HttpPost("join/{channelId}")]
        public async Task<IActionResult> JoinUserToGroup([FromRoute] Guid channelId)
        {
            try
            {
                var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var channel = await _service.GetChannelAsync(channelId);

                if (channel is null) { NotFound("Channel has not been found it"); }

                Guid.TryParse(user, out Guid userId);

                await _channelMembers.JoinToChannel(userId, channelId);
                return Ok();
            }catch(Exception ex)
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
            bool ok = await _service.UpdateChannelAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool ok = await _service.DeleteChannelAsync(id);
            return ok ? NoContent() : NotFound();
        }

    }

}
