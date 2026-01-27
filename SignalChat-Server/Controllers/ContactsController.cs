using Application.Common.Interfaces.Contacts;
using Application.Common.Interfaces.Utils;
using Application.DTOs.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SignalChat_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly IContactsService _contactsService;
        private readonly ICurrentUserService _currentUserService;

        public ContactsController(ILogger<ContactsController> logger,
            IContactsService contactsService,
            ICurrentUserService currentUserService)
        {

            _logger = logger;
            _contactsService = contactsService;
            _currentUserService = currentUserService;

        }

        [HttpPost]
        public async Task<IActionResult> AddNewContact([FromBody] AddContactDto request)
        {
            if (string.IsNullOrEmpty(request.username)) return BadRequest("User name is required");
            await _contactsService.AddAsync(_currentUserService.UserId, request.username);
            return Ok("Request sent correctly");
        }
        [HttpPut("approve/{Id}")]
        public async Task<IActionResult> Approve([FromRoute] string Id)
        {
            if (string.IsNullOrWhiteSpace(Id)) return BadRequest("Incorrect id");
            Guid.TryParse(Id, out Guid requesterId);
            await _contactsService.ApproveAsync(requesterId, _currentUserService.UserId);
            return Ok("Contact added correctly");

        }
        [HttpPut("reject/{Id}")]
        public async Task<IActionResult> Reject([FromRoute] string Id)
        {
            if (string.IsNullOrWhiteSpace(Id)) return BadRequest("Incorrect id");
            Guid.TryParse(Id, out Guid requesterId);
            await _contactsService.RejectAsync(requesterId, _currentUserService.UserId);
            return Ok("Contect rejected correctly");

        }
        [HttpGet("approved")]
        public async Task<IActionResult> GetApproved()
        {
            if (_currentUserService.UserId == Guid.Empty) return Unauthorized();
            var response = await _contactsService.GetApprovedAsync(_currentUserService.UserId);
            return Ok(response);
        }
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            if (_currentUserService.UserId == Guid.Empty) return Unauthorized();
            var response = await _contactsService.GetPendingAsync(_currentUserService.UserId);
            return Ok(response);
        }
    }
}
