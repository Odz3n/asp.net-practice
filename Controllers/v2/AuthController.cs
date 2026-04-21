using System.Security.Cryptography.X509Certificates;
using Asp.Versioning;
using hw_2_2_3_26.Features.Auth.Commands.Login;
using hw_2_2_3_26.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Practice.DTO;
using Practice.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Practice.Controllers.V2;

[ApiVersion(2.0)]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[SwaggerTag("Provides API endpoints for managing Authentication.")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;
    public AuthController(
        IAuthService authService,
        IMediator mediator)
    {
        _authService = authService;
        _mediator = mediator;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return StatusCode(201, result);
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}