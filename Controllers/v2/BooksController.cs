using Asp.Versioning;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using hw_2_2_3_26.Services;
using Swashbuckle.AspNetCore.Annotations;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Validators;

namespace hw_2_2_3_26.Controllers.v2
{
    /// <summary>
    /// Provides API endpoints for managing books.
    /// </summary>
    /// <remarks>
    /// This controller allows clients to retrieve, search, create,
    /// update, partially update, and delete books stored in the database.
    /// API Version: 2.0
    /// </remarks>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [SwaggerTag("Provides API endpoints for managing books.")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController"/> class.
        /// </summary>
        /// <param name="bookService">Service for book-related operations.</param>
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Retrieves the complete list of books.
        /// </summary>
        /// <param name="parameters">Get parameters for book.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of <see cref="BookSummaryDto"/> objects.</returns>
        /// <response code="200">Returns the list of books.</response>
        [HttpGet]
        public async Task<ActionResult<PagedResult<BookSummaryDto>>> Get(
            [FromQuery] BookGetParameters parameters,
            CancellationToken ct)
        {
            var res = await _bookService.GetAllBooks(parameters, ct);
            return Ok(res);
        }

        /// <summary>
        /// Retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The matching <see cref="BookDetailDto"/> if found.</returns>
        /// <response code="200">The book was successfully found.</response>
        /// <response code="404">No book with the specified id exists.</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDetailDto>> GetById(int id, CancellationToken ct)
        {
            var target = await _bookService.GetBookById(id, ct);
            return Ok(target);
        }

        /// <summary>
        /// Searches for books using the provided query parameters.
        /// </summary>
        /// <param name="parameters">Search parameters (e.g., title, author).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of books matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching books.</response>
        /// <response code="400">Required search parameters were not provided.</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDetailDto>>> GetBySearchParams(
            [FromQuery] BookSearchParameters parameters,
            CancellationToken ct)
        {
            var result = await _bookService.GetBooksByTitleAndAuthor(parameters, ct);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="request">The data required to create a new book.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created book.</returns>
        /// <response code="201">Book was successfully created.</response>
        /// <response code="400">Invalid book data was provided.</response>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<BookDetailDto>> Create(
            [FromForm] CreateBookRequest request,
            CancellationToken ct)
        {
            var bookDetail = await _bookService.Create(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = bookDetail.Id }, bookDetail);
        }

        /// <summary>
        /// Fully updates an existing book by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the book to update.</param>
        /// <param name="request">The updated book data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Book was successfully updated.</response>
        /// <response code="400">Invalid book data was provided.</response>
        /// <response code="404">Book with the specified id was not found.</response>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] CreateBookRequest request,
            CancellationToken ct)
        {
            await _bookService.Update(id, request, ct);
            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing book by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the book to update.</param>
        /// <param name="request">The partial update data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Book was successfully updated.</response>
        /// <response code="400">Invalid book data was provided.</response>
        /// <response code="404">Book with the specified id was not found.</response>
        [HttpPatch("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PartialUpdate(
            int id,
            [FromForm] UpdateBookRequest request,
            CancellationToken ct)
        {
            await _bookService.PartialUpdate(id, request, ct);
            return NoContent();
        }

        /// <summary>
        /// Deletes a book by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the book to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the deletion was successful.</returns>
        /// <response code="204">Book was successfully deleted.</response>
        /// <response code="404">Book with the specified id was not found.</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            await _bookService.Delete(id, ct);
            return NoContent();
        }
    }
}