using Microsoft.AspNetCore.Identity;
using Practice.DTO;

namespace Practice.Services;

public interface IRoleService
{
    Task<IdentityResult> CreateRoleAsync(RoleDto role);
    Task<IdentityResult> DeleteRoleAsync(string roleName);
    List<string> GetAllRoles();
    Task<IdentityResult> AssignRoleAsync(UserRoleDto userRole);
    Task<IdentityResult> RemoveRoleAsync(UserRoleDto userRole);
    Task<IList<string>> GetUserRolesAsync(string userId);
}