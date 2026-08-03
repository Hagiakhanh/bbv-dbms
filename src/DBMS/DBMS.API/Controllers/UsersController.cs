using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.DTOs.Users;
using DBMS.API.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var created = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { userId = created.UserId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {userId} not found." });
        }
        return Ok(user);
    }

    [HttpPatch("{userId:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var updated = await _userService.UpdateUserAsync(userId, request);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var deleted = await _userService.DeleteUserAsync(userId);
        if (!deleted)
        {
            return NotFound(new { Message = $"User with ID {userId} not found." });
        }
        return NoContent();
    }
}
