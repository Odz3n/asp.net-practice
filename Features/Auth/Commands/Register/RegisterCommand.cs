using MediatR;
using Microsoft.AspNetCore.Identity;

namespace hw_2_2_3_26.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password
): IRequest<IdentityResult>;