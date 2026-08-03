using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DBMS.API.DTOs.Roles;
using DBMS.API.Services.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API.Controllers;

[ApiController]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("api/v1/roles")]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            var created = await _roleService.CreateRoleAsync(request);
            return CreatedAtAction(nameof(GetRoleById), new { roleId = created.RoleId }, created);
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

    [HttpGet("api/v1/roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("api/v1/roles/{roleId:int}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(int roleId)
    {
        var role = await _roleService.GetRoleByIdAsync(roleId);
        if (role == null)
        {
            return NotFound(new { Message = $"Role with ID {roleId} not found." });
        }
        return Ok(role);
    }

    [HttpPost("api/v1/users/{userId:int}/roles")]
    public async Task<IActionResult> AssignRolesToUser(int userId, [FromBody] AssignRoleRequest request)
    {
        try
        {
            await _roleService.AssignRolesToUserAsync(userId, request.Roles);
            return Ok(new { Message = "Roles assigned successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpDelete("api/v1/users/{userId:int}/roles/{roleId:int}")]
    public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
    {
        try
        {
            await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPost("api/v1/roles/{roleId:int}/permissions")]
    public async Task<ActionResult<RoleDto>> GrantPermissionsToRole(int roleId, [FromBody] GrantPermissionRequest request)
    {
        try
        {
            var updated = await _roleService.GrantPermissionsToRoleAsync(roleId, request.Permissions);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpDelete("api/v1/roles/{roleId:int}/permissions/{permission}")]
    public async Task<ActionResult<RoleDto>> RevokePermissionFromRole(int roleId, string permission)
    {
        try
        {
            var updated = await _roleService.RevokePermissionFromRoleAsync(roleId, permission);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
