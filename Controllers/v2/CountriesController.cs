using Asp.Versioning;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace hw_2_2_3_26.Controllers.v2
{
    /// <summary>
    /// Provides API endpoints for managing countries.
    /// </summary>
    /// <remarks>
    /// This controller allows clients to retrieve, search, create,
    /// update, partially update, and delete countries stored in the database.
    /// API Version: 2.0
    /// </remarks>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [SwaggerTag("Provides API endpoints for managing countries.")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountriesController"/> class.
        /// </summary>
        /// <param name="countryService">Service for country-related operations.</param>
        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        /// <summary>
        /// Retrieves the complete list of countries.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of <see cref="CountrySummaryDto"/> objects.</returns>
        /// <response code="200">Returns the list of countries.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountrySummaryDto>>> Get(CancellationToken ct)
        {
            var result = await _countryService.GetAllCountries(ct);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a country by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the country.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The matching <see cref="CountryDetailDto"/> if found.</returns>
        /// <response code="200">The country was successfully found.</response>
        /// <response code="404">No country with the specified id exists.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CountryDetailDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _countryService.GetCountryById(id, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Searches for countries using the provided query parameters.
        /// </summary>
        /// <param name="parameters">Search parameters (e.g., name).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of countries matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching countries.</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CountryDetailDto>>> GetBySearchParameters(
            [FromQuery] CountrySearchParameters parameters,
            CancellationToken ct)
        {
            var result = await _countryService.GetCountryBySearchParameters(parameters, ct);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new country.
        /// </summary>
        /// <param name="request">The data required to create a new country.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created country.</returns>
        /// <response code="200">Country was successfully created.</response>
        /// <response code="400">Invalid country data was provided.</response>
        [HttpPost]
        public async Task<ActionResult<CountrySummaryDto>> Create(
            [FromBody] CreateCountryRequest request,
            CancellationToken ct)
        {
            var result = await _countryService.Create(request, ct);
            return result;
        }

        /// <summary>
        /// Deletes a country by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the country to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">Country was successfully deleted.</response>
        /// <response code="404">Country with the specified id was not found.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            var result = await _countryService.Delete(id, ct);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Fully updates an existing country by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the country to update.</param>
        /// <param name="request">The updated country data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Country was successfully updated.</response>
        /// <response code="404">Country with the specified id was not found.</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateCountryRequest request,
            CancellationToken ct)
        {
            var result = await _countryService.Update(id, request, ct);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing country by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the country to update.</param>
        /// <param name="request">The partial update data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Country was successfully updated.</response>
        /// <response code="404">Country with the specified id was not found.</response>
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartialUpdate(
            int id,
            [FromBody] PartialUpdateCountryRequest request,
            CancellationToken ct)
        {
            var result = await _countryService.PartialUpdate(id, request, ct);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}