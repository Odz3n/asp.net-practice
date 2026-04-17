using hw_2_2_3_26.Models;
using hw_2_2_3_26.Services;
using Microsoft.AspNetCore.Identity;
using Practice.DTO;

namespace Practice.Services;

public class AuthService: IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    public AuthService(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string?> LoginAsync(LoginDto login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user == null)
            return null;

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, login.Password);
        if (!isPasswordCorrect)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return _tokenService.GenerateToken(user, roles);
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto register)
    {
        var user = new ApplicationUser
        {
            UserName = register.Email,
            Email = register.Email
        };

        return await _userManager.CreateAsync(user, register.Password);
    }
}