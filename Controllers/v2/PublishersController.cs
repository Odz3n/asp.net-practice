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
    /// Provides API endpoints for managing publishers.
    /// </summary>
    /// <remarks>
    /// This controller allows clients to retrieve, search, create,
    /// update, partially update, and delete publishers stored in the database.
    /// API Version: 2.0
    /// </remarks>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [SwaggerTag("Provides API endpoints for managing publishers.")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishersController"/> class.
        /// </summary>
        /// <param name="publisherService">Service for publisher-related operations.</param>
        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        /// <summary>
        /// Retrieves the complete list of publishers.
        /// </summary>
        /// <param name="parameters">Publisher get parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of <see cref="PublisherSummaryDto"/> objects.</returns>
        /// <response code="200">Returns the list of publishers.</response>
        [HttpGet]
        public async Task<ActionResult<PagedResult<PublisherSummaryDto>>> Get([FromQuery] PublisherGetParameters parameters, CancellationToken ct)
        {
            var result = await _publisherService.GetAllPublishers(parameters, ct);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a publisher by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the publisher.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The matching <see cref="PublisherDetailDto"/> if found.</returns>
        /// <response code="200">The publisher was successfully found.</response>
        /// <response code="404">No publisher with the specified id exists.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PublisherDetailDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _publisherService.GetPublisherById(id, ct);
            return Ok(result);
        }

        /// <summary>
        /// Searches for publishers using the provided query parameters.
        /// </summary>
        /// <param name="parameters">Search parameters (e.g., name, country).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of publishers matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching publishers.</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PublisherDetailDto>>> GetBySearchParameters(
            [FromQuery] PublisherSearchParameters parameters,
            CancellationToken ct)
        {
            var result = await _publisherService.GetPublisherBySearchParameters(parameters, ct);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new publisher.
        /// </summary>
        /// <param name="request">The data required to create a new publisher.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created publisher.</returns>
        /// <response code="200">Publisher was successfully created.</response>
        /// <response code="400">Invalid publisher data was provided.</response>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PublisherSummaryDto>> Create(
            [FromForm] CreatePublisherRequest request,
            CancellationToken ct)
        {
            var result = await _publisherService.Create(request, ct);
            return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
        }

        /// <summary>
        /// Deletes a publisher by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the publisher to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">Publisher was successfully deleted.</response>
        /// <response code="404">Publisher with the specified id was not found.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            await _publisherService.Delete(id, ct);
            return NoContent();
        }

        /// <summary>
        /// Fully updates an existing publisher by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the publisher to update.</param>
        /// <param name="request">The updated publisher data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Publisher was successfully updated.</response>
        /// <response code="404">Publisher with the specified id was not found.</response>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdatePublisherRequest request,
            CancellationToken ct)
        {
            await _publisherService.Update(id, request, ct);
            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing publisher by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the publisher to update.</param>
        /// <param name="request">The partial update data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Publisher was successfully updated.</response>
        /// <response code="404">Publisher with the specified id was not found.</response>
        [HttpPatch("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PartialUpdate(
            int id,
            [FromForm] PartialUpdatePublisherRequest request,
            CancellationToken ct)
        {
            await _publisherService.PartialUpdate(id, request, ct);
            return NoContent();
        }
    }
}