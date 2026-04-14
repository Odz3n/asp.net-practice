using hw_2_2_3_26.Models;
using Microsoft.AspNetCore.Identity;
using Practice.DTO;

namespace Practice.Services;

public class AuthService: IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AuthService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<SignInResult> LoginAsync(LoginDto login)
    {
        Console.WriteLine("\n\n");
        
        Console.WriteLine(login.Email);
        Console.WriteLine(login.Password);
        
        Console.WriteLine("\n\n");
        return await _signInManager.PasswordSignInAsync(
            login.Email,
            login.Password,
            isPersistent: false,
            lockoutOnFailure: false);
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