using Asp.Versioning;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace hw_2_2_3_26.Controllers.v4
{
    /// <summary>
    /// Provides API endpoints for managing genres.
    /// </summary>
    /// <remarks>
    /// This controller allows clients to retrieve, search, create,
    /// update, partially update, and delete genres stored in the database.
    /// API Version: 4.0
    /// </remarks>
    [ApiVersion("4.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [SwaggerTag("Provides API endpoints for managing genres.")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenresController"/> class.
        /// </summary>
        /// <param name="genreService">Service for genre-related operations.</param>
        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        /// <summary>
        /// Retrieves the complete list of genres.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of <see cref="GenreSummaryDto"/> objects.</returns>
        /// <response code="200">Returns the list of genres.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreSummaryDto>>> Get(CancellationToken ct)
        {
            var result = await _genreService.GetAllGenres(ct);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a genre by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the genre.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The matching <see cref="GenreDetailDto"/> if found.</returns>
        /// <response code="200">The genre was successfully found.</response>
        /// <response code="404">No genre with the specified id exists.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GenreDetailDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _genreService.GetGenreById(id, ct);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Searches for genres using the provided query parameters.
        /// </summary>
        /// <param name="parameters">Search parameters (e.g., name).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of genres matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching genres.</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GenreDetailDto>>> GetBySearchParameters(
            [FromQuery] GenreSearchParameters parameters,
            CancellationToken ct)
        {
            var result = await _genreService.GetGenreBySearchParameters(parameters, ct);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new genre.
        /// </summary>
        /// <param name="request">The data required to create a new genre.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created genre.</returns>
        /// <response code="200">Genre was successfully created.</response>
        /// <response code="400">Invalid genre data was provided.</response>
        [HttpPost]
        public async Task<ActionResult<GenreSummaryDto>> Create(
            [FromBody] CreateGenreRequest request,
            CancellationToken ct)
        {
            var result = await _genreService.Create(request, ct);
            return result;
        }

        /// <summary>
        /// Deletes a genre by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the genre to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">Genre was successfully deleted.</response>
        /// <response code="404">Genre with the specified id was not found.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            var result = await _genreService.Delete(id, ct);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Fully updates an existing genre by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the genre to update.</param>
        /// <param name="request">The updated genre data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Genre was successfully updated.</response>
        /// <response code="404">Genre with the specified id was not found.</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateGenreRequest request,
            CancellationToken ct)
        {
            var result = await _genreService.Update(id, request, ct);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing genre by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the genre to update.</param>
        /// <param name="request">The partial update data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Genre was successfully updated.</response>
        /// <response code="404">Genre with the specified id was not found.</response>
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartialUpdate(
            int id,
            [FromBody] PartialUpdateGenreRequest request,
            CancellationToken ct)
        {
            var result = await _genreService.PartialUpdate(id, request, ct);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}