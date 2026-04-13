using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace hw_2_2_3_26.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class WeatherController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetWeatherToday()
    {
        return Ok(new
        {
            Message = "Сьогодні гарна погода",
            Temperature = 18,
            Timestamp = DateTime.UtcNow
        });
    }
}