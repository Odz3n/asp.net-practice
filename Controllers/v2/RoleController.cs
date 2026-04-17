// Controllers/RoleController.cs
using Microsoft.AspNetCore.Mvc;
using Practice.Services;
using Asp.Versioning;
using Practice.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Practice.Controllers.V2;

[ApiVersion(2.0)]
[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole(RoleDto role)
    {
        var result = await _roleService.CreateRoleAsync(role);

        if (result.Succeeded)
            return Ok($"Роль '{role.RoleName}' створена");

        return BadRequest(result.Errors);
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var result = await _roleService.DeleteRoleAsync(roleName);

        if (result.Succeeded)
            return Ok($"Роль '{roleName}' видалена");

        return BadRequest(result.Errors);
    }

    [HttpGet]
    public IActionResult GetAllRoles()
    {
        return Ok(_roleService.GetAllRoles());
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(UserRoleDto userRole)
    {
        var result = await _roleService.AssignRoleAsync(userRole);

        if (result.Succeeded)
            return Ok($"Роль '{userRole.RoleName}' призначена");

        return BadRequest(result.Errors);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRole(UserRoleDto userRole)
    {
        var result = await _roleService.RemoveRoleAsync(userRole);

        if (result.Succeeded)
            return Ok($"Роль '{userRole.RoleName}' забрана");

        return BadRequest(result.Errors);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var roles = await _roleService.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}