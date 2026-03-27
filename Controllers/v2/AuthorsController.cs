using Asp.Versioning;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace hw_2_2_3_26.Controllers.v2
{
    /// <summary>
    /// Provides API endpoints for managing authors.
    /// </summary>
    /// <remarks>
    /// Supports retrieving, searching, creating, updating, and deleting authors.
    /// API Version: 2.0
    /// </remarks>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [SwaggerTag("Provides API endpoints for managing authors.")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorsController"/> class.
        /// </summary>
        /// <param name="authorService">Service for author-related operations.</param>
        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        /// <summary>
        /// Retrieves all authors.
        /// </summary>
        /// <param name="parameters">Author get parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of author summary DTOs.</returns>
        /// <response code="200">Returns the list of authors.</response>
        [HttpGet]
        public async Task<ActionResult<PagedResult<AuthorSummaryDto>>> Get([FromQuery] AuthorGetParameters parameters, CancellationToken ct)
        {
            var result = await _authorService.GetAllAuthors(parameters, ct);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves an author by ID.
        /// </summary>
        /// <param name="id">Author identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Detailed author information.</returns>
        /// <response code="200">Returns the requested author.</response>
        /// <response code="404">If the author is not found.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDetailDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _authorService.GetAuthorById(id, ct);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Searches authors using query parameters.
        /// </summary>
        /// <param name="parameters">Search parameters (e.g., name, country).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A filtered collection of authors.</returns>
        /// <response code="200">Returns matching authors.</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuthorDetailDto>>> GetBySearchParameters(
            [FromQuery] AuthorSearchParameters parameters,
            CancellationToken ct)
        {
            var result = await _authorService.GetAuthorBySearchParameters(parameters, ct);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new author.
        /// </summary>
        /// <param name="request">Author creation data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created author summary.</returns>
        /// <response code="200">Returns the created author.</response>
        [HttpPost]
        public async Task<ActionResult<AuthorSummaryDto>> Create(
            [FromBody] CreateAuthorRequest request,
            CancellationToken ct)
        {
            var result = await _authorService.Create(request, ct);
            return result;
        }

        /// <summary>
        /// Deletes an author by ID.
        /// </summary>
        /// <param name="id">Author identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Author deleted successfully.</response>
        /// <response code="404">If the author is not found.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            var result = await _authorService.Delete(id, ct);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Fully updates an existing author.
        /// </summary>
        /// <param name="id">Author identifier.</param>
        /// <param name="request">Updated author data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Author updated successfully.</response>
        /// <response code="404">If the author is not found.</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateAuthorRequest request,
            CancellationToken ct)
        {
            var result = await _authorService.Update(id, request, ct);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing author.
        /// </summary>
        /// <param name="id">Author identifier.</param>
        /// <param name="request">Partial update data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Author updated successfully.</response>
        /// <response code="404">If the author is not found.</response>
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartialUpdate(
            int id,
            [FromBody] PartialUpdateAuthorRequest request,
            CancellationToken ct)
        {
            var result = await _authorService.PartialUpdate(id, request, ct);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}