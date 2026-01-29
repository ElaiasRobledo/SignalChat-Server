using Application.Common.Interfaces.Users;
using Application.Common.Interfaces.Utils;
using Application.DTOs.Contacts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SignalChat_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UsersController> _logger;  
        public UsersController(IUserService userService, ILogger<UsersController> logger,
            ICurrentUserService currentUserService)
        {
            _userService = userService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] AddContactDto request)
        {
            var result = await _userService.GetAllAsync(_currentUserService.UserId,request.username);
            return Ok(result);  
            
        }    
    }
}
