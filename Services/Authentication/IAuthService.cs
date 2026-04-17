using Microsoft.AspNetCore.Identity;
using Practice.DTO;

namespace Practice.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterDto register);
    Task<string?> LoginAsync(LoginDto login);
}