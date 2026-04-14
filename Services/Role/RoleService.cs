using hw_2_2_3_26.Models;
using Microsoft.AspNetCore.Identity;
using Practice.DTO;


namespace Practice.Services;

public class RoleService: IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<IdentityRole> roleManager,
                       UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateRoleAsync(RoleDto role)
    {
        if (await _roleManager.RoleExistsAsync(role.RoleName))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Роль '{role.RoleName}' вже існує"
            });

        return await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Роль '{roleName}' не знайдена"
            });

        return await _roleManager.DeleteAsync(role);
    }

    public List<string> GetAllRoles()
    {
        return _roleManager.Roles.Select(r => r.Name!).ToList();
    }

    public async Task<IdentityResult> AssignRoleAsync(UserRoleDto userRole)
    {
        var user = await _userManager.FindByIdAsync(userRole.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Користувача не знайдено"
            });

        if (!await _roleManager.RoleExistsAsync(userRole.RoleName))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Роль '{userRole.RoleName}' не існує"
            });

        return await _userManager.AddToRoleAsync(user, userRole.RoleName);
    }

    public async Task<IdentityResult> RemoveRoleAsync(UserRoleDto userRole)
    {
        var user = await _userManager.FindByIdAsync(userRole.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Користувача не знайдено"
            });

        return await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }
}