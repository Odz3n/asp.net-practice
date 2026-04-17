using Asp.Versioning;
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
    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto register)
    {
        var result = await _authService.RegisterAsync(register);
        if (result.Succeeded)
            return Ok(new { message = "Successfully registered!" });
        return BadRequest(new
        {
            message = "Registration error!",
            errors = result.Errors
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto login)
    {
        var token = await _authService.LoginAsync(login);
        if (token != null)
            return Ok(new { token });
        return Unauthorized(new
        {
            message = "Login error! Invalid email or password.",
        });
    }
}