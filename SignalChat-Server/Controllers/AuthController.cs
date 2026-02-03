using Application.Common.Interfaces.Users;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using static Application.DTOs.Users.AuthUserDto;

namespace SignalChat_Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUser _authUser;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthUser authUser, ILogger<AuthController> logger)
        {
            _authUser = authUser;
            _logger = logger;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] RegisterUserDto request)
        {
            try
            {
                await _authUser.AddUserAsync(request);
                _logger.LogInformation("User created");
                return Created();
            }
            catch (UserAlreadyExistsException ex)
            {
                _logger.LogError($"Register user: {ex.Message}");
                return BadRequest(ex.Message);
            }

        }
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto request)
        {
            try
            {
                var result = await _authUser.VerifyUserAsync(request);
                _logger.LogInformation("User loged");
                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                _logger.LogWarning($"Login user: {ex.Message}");
                return Unauthorized(new { message = ex.Message});
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogWarning($"Login user: {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }

        }
    }
}
