using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.DTOs.Auth;
using OnlineStore.DTOs.Common;
using OnlineStore.Services.Auth;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email/username or password." });
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result == null)
            {
                return BadRequest(new { message = "Invalid or expired access token / refresh token." });
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<MessageResponse>> Logout([FromQuery] bool allDevices = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub")
                      ?? User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _authService.LogoutAsync(userId, allDevices);
            return Ok(result);
        }
    }
}
