using hw_2_2_3_26.Models;
using hw_2_2_3_26.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace hw_2_2_3_26.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    public async Task<string?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return string.Empty;

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordCorrect)
            return string.Empty;

        var roles = await _userManager.GetRolesAsync(user);

        return _tokenService.GenerateToken(user, roles);
    }
}