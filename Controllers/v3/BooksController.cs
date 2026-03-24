using Asp.Versioning;
using AutoMapper;
using hw_2_2_3_26.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MyApp.Controllers.v3
{
    /// <summary>
    /// Provides API endpoints for managing books (API Version 3).
    /// </summary>
    /// <remarks>
    /// This controller allows clients to retrieve, search, create, update,
    /// and delete books stored in the in-memory collection.
    /// </remarks>
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [SwaggerTag("Provides API endpoints for managing books.")]
    public class BooksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;
        public BooksController(IMapper mapper, AppDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        /// <summary>
        /// Retrieves the complete list of books.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of <see cref="BookSummaryDto"/> objects.</returns>
        /// <response code="200">Returns the list of books.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookSummaryDto>>> GetBooks(CancellationToken ct)
        {
            var books = await _db.Books.AsNoTracking().ToListAsync(ct);
            var summary = _mapper.Map<IEnumerable<BookSummaryDto>>(books);
            return Ok(summary);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDetailDto>> GetById(int id, CancellationToken ct)
        {
            var target = await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);

            if (target == null)
                return NotFound("Book not found.");

            var detail = _mapper.Map<BookDetailDto>(target);
            return Ok(detail);
        }

        /// <summary>
        /// Searches for books by title and optionally by author.
        /// </summary>
        /// <param name="title">The title or part of the title to search for.</param>
        /// <param name="author">Optional author name used to further filter the results.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of books matching the search criteria.</returns>
        /// <remarks>
        /// Title is required for the search operation.  
        /// Author is optional and will be used as an additional filter if provided.
        /// </remarks>
        /// <response code="200">Returns the list of matching books.</response>
        /// <response code="400">Title parameter was not provided.</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BookDetailDto>>> GetByTitleAndAuthor([FromQuery] string? title, [FromQuery] string? author, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest(new { error = "Search target title must be provided." });

            var query = _db.Books
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Contains(title.ToLower()));

            // if (!string.IsNullOrWhiteSpace(author))
            // {
            //     query = query.Where(b =>
            //         (b.AuthorFirstName + " " + b.AuthorLastName)
            //         .ToLower()
            //         .Contains(author.ToLower()));
            // }

            var books = await query.ToListAsync(ct);

            var result = _mapper.Map<IEnumerable<BookDetailDto>>(books);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="book">The book object containing data for the new book.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created <see cref="Book"/>.</returns>
        /// <response code="201">Book was successfully created.</response>
        /// <response code="400">Invalid book data was provided.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDetailDto>> Create([FromBody] CreateBookRequest book, CancellationToken ct)
        {
            if (book == null)
                return BadRequest(new { error = "Book's data must be provided." });

            var newBook = _mapper.Map<Book>(book);

            await _db.Books.AddAsync(newBook, ct);
            await _db.SaveChangesAsync(ct);

            var bookDetail = _mapper.Map<BookDetailDto>(newBook);
            return CreatedAtAction(nameof(GetById), new { id = bookDetail.Id }, bookDetail);
        }

        /// <summary>
        /// Updates an existing book by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the book to update.</param>
        /// <param name="book">The updated book data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Book was successfully updated.</response>
        /// <response code="400">Invalid book data was provided.</response>
        /// <response code="404">Book with the specified id was not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUpdateById(int id, [FromBody] CreateBookRequest book, CancellationToken ct)
        {
            if (book == null)
                return BadRequest("Book data must be provided.");

            var target = await _db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

            if (target == null)
                return NotFound("Book not found.");

            _mapper.Map(book, target);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing book by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the book to update.</param>
        /// <param name="book">The updated book data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if the update was successful.</returns>
        /// <response code="204">Book was successfully updated.</response>
        /// <response code="400">Invalid book data was provided.</response>
        /// <response code="404">Book with the specified id was not found.</response>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchUpdateById(int id, [FromBody] UpdateBookRequest book, CancellationToken ct)
        {
            if (book == null)
                return BadRequest("Book data must be provided.");

            var target = await _db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

            if (target == null)
                return NotFound("Book not found.");

            _mapper.Map(book, target);
            await _db.SaveChangesAsync(ct);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            var target = await _db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

            if (target == null)
                return NotFound("Book not found.");

            _db.Books.Remove(target);
            await _db.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}