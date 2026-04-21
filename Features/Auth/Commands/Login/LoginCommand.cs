using MediatR;
namespace hw_2_2_3_26.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<string?>;