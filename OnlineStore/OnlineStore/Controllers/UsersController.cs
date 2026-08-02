using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.DTOs.Auth;
using OnlineStore.Services.Users;

namespace OnlineStore.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;

        public UsersController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Display the current user's name, email, avatar, role, and store
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser(
            [FromQuery] bool includeRole = true,
            [FromQuery] bool includeStore = true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                      ?? User.FindFirstValue("sub")
                      ?? User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _currentUserService.GetCurrentUserAsync(userId, includeRole, includeStore);
            if (result == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(result);
        }
    }
}
