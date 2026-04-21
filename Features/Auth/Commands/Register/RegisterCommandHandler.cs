using hw_2_2_3_26.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace hw_2_2_3_26.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IdentityResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<IdentityResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        return await _userManager.CreateAsync(user, request.Password);
    }
}