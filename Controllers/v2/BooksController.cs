// using Asp.Versioning;
// using AutoMapper;
// using Microsoft.AspNetCore.Mvc;
// using MyApp.Models;
// using Swashbuckle.AspNetCore.Annotations;

// namespace MyApp.Controllers.v2
// {
//     /// <summary>
//     /// Provides API endpoints for managing books (API Version 2).
//     /// </summary>
//     /// <remarks>
//     /// This controller allows clients to retrieve, search, create, update,
//     /// and delete books stored in the in-memory collection.
//     /// </remarks>
//     [ApiVersion("2.0")]
//     [Route("api/v{version:apiVersion}/[controller]")]
//     [ApiController]
//     [Produces("application/json")]
//     [SwaggerTag("Provides API endpoints for managing books.")]
//     public class BooksController : ControllerBase
//     {
//         private readonly IMapper _mapper;
//         public BooksController(IMapper mapper)
//         {
//             _mapper = mapper;
//         }
//         private static List<Book> _books = new()
//         {
//             new Book { Id = 0, Title = "1984", AuthorFirstName = "George", AuthorLastName = "Orwell", Year = 1949, Genre = "Dystopian", PageCount = 328 },
//             new Book { Id = 1, Title = "Animal Farm", AuthorFirstName = "George", AuthorLastName = "Orwell", Year = 1945, Genre = "Political Satire", PageCount = 112 },
//             new Book { Id = 2, Title = "The Hobbit", AuthorFirstName = "J.R.R.", AuthorLastName = "Tolkien", Year = 1937, Genre = "Fantasy", PageCount = 310 },
//             new Book { Id = 3, Title = "The Lord of the Rings: The Fellowship of the Ring", AuthorFirstName = "J.R.R.", AuthorLastName = "Tolkien", Year = 1954, Genre = "Fantasy", PageCount = 423 },
//             new Book { Id = 4, Title = "The Lord of the Rings: The Two Towers", AuthorFirstName = "J.R.R.", AuthorLastName = "Tolkien", Year = 1954, Genre = "Fantasy", PageCount = 352 },
//             new Book { Id = 5, Title = "The Lord of the Rings: The Return of the King", AuthorFirstName = "J.R.R.", AuthorLastName = "Tolkien", Year = 1955, Genre = "Fantasy", PageCount = 416 },
//             new Book { Id = 6, Title = "Fahrenheit 451", AuthorFirstName = "Ray", AuthorLastName = "Bradbury", Year = 1953, Genre = "Dystopian", PageCount = 194 },
//             new Book { Id = 7, Title = "Brave New World", AuthorFirstName = "Aldous", AuthorLastName = "Huxley", Year = 1932, Genre = "Science Fiction", PageCount = 311 },
//             new Book { Id = 8, Title = "The Catcher in the Rye", AuthorFirstName = "J.D.", AuthorLastName = "Salinger", Year = 1951, Genre = "Literary Fiction", PageCount = 277 },
//             new Book { Id = 9, Title = "Harry Potter and the Philosopher's Stone", AuthorFirstName = "J.K.", AuthorLastName = "Rowling", Year = 1997, Genre = "Fantasy", PageCount = 223 },
//             new Book { Id = 10, Title = "Harry Potter and the Chamber of Secrets", AuthorFirstName = "J.K.", AuthorLastName = "Rowling", Year = 1998, Genre = "Fantasy", PageCount = 251 },
//             new Book { Id = 11, Title = "Harry Potter and the Prisoner of Azkaban", AuthorFirstName = "J.K.", AuthorLastName = "Rowling", Year = 1999, Genre = "Fantasy", PageCount = 317 },
//             new Book { Id = 12, Title = "The Silmarillion", AuthorFirstName = "J.R.R.", AuthorLastName = "Tolkien", Year = 1977, Genre = "Fantasy", PageCount = 365 },
//             new Book { Id = 13, Title = "The Old Man and the Sea", AuthorFirstName = "Ernest", AuthorLastName = "Hemingway", Year = 1952, Genre = "Literary Fiction", PageCount = 127 }
//         };

//         /// <summary>
//         /// Retrieves the complete list of books.
//         /// </summary>
//         /// <returns>A collection of <see cref="BookSummaryDto"/> objects.</returns>
//         /// <response code="200">Returns the list of books.</response>
//         [HttpGet]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         public ActionResult<IEnumerable<BookSummaryDto>> GetBooks()
//         {
//             var summary = _mapper.Map<IEnumerable<BookSummaryDto>>(_books);
//             return Ok(summary);
//         }

//         /// <summary>
//         /// Retrieves a book by its unique identifier.
//         /// </summary>
//         /// <param name="id">The unique identifier of the book.</param>
//         /// <returns>The matching <see cref="BookDetailDto"/> if found.</returns>
//         /// <response code="200">The book was successfully found.</response>
//         /// <response code="404">No book with the specified id exists.</response>
//         [HttpGet("{id:int}")]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public ActionResult<BookDetailDto> GetById(int id)
//         {
//             var target = _books.FirstOrDefault(b => b.Id == id);

//             if (target == null)
//                 return NotFound("Book not found.");

//             var detail = _mapper.Map<BookDetailDto>(target);
//             return Ok(detail);
//         }

//         /// <summary>
//         /// Searches for books by title and optionally by author.
//         /// </summary>
//         /// <param name="title">The title or part of the title to search for.</param>
//         /// <param name="author">
//         /// Optional author name used to further filter the results.
//         /// </param>
//         /// <returns>A collection of books matching the search criteria.</returns>
//         /// <remarks>
//         /// Title is required for the search operation.  
//         /// Author is optional and will be used as an additional filter if provided.
//         /// </remarks>
//         /// <response code="200">Returns the list of matching books.</response>
//         /// <response code="400">Title parameter was not provided.</response>
//         [HttpGet("search")]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         public ActionResult<IEnumerable<BookDetailDto>> GetByTitleAndAuthor([FromQuery] string? title, [FromQuery] string? author)
//         {
//             if (string.IsNullOrWhiteSpace(title))
//                 return BadRequest("Search target title must be provided.");

//             var query = _books.Where(b =>
//                 b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
//             var res = _mapper.Map<IEnumerable<BookDetailDto>>(query);

//             if (!string.IsNullOrWhiteSpace(author))
//             {
//                 var books = res.Where(b =>
//                     b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));

//                 return Ok(books);
//             }

//             return Ok(res);
//         }

//         /// <summary>
//         /// Creates a new book.
//         /// </summary>
//         /// <param name="book">The book object containing data for the new book.</param>
//         /// <returns>The newly created <see cref="Book"/>.</returns>
//         /// <response code="201">Book was successfully created.</response>
//         /// <response code="400">Invalid book data was provided.</response>
//         [HttpPost]
//         [ProducesResponseType(StatusCodes.Status201Created)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         public ActionResult<BookDetailDto> Create([FromBody] CreateBookRequest book)
//         {
//             if (book == null)
//                 return BadRequest("Book's data must be provided.");

//             var newBook = new Book
//             {
//                 Id = _books.Max(b => b.Id) + 1,
//                 Title = book.Title,
//                 AuthorFirstName = book.Author.Split(' ')[0],
//                 AuthorLastName = book.Author.Split(' ')[1],
//                 Year = book.Year,
//                 Genre = book.Genre,
//                 PageCount = book.PageCount
//             };

//             _books.Add(newBook);

//             var bookDetail = _mapper.Map<BookDetailDto>(newBook);

//             return CreatedAtAction(nameof(GetById), new { Id = bookDetail.Id }, bookDetail);
//         }

//         /// <summary>
//         /// Updates an existing book by its identifier.
//         /// </summary>
//         /// <param name="id">The identifier of the book to update.</param>
//         /// <param name="book">The updated book data.</param>
//         /// <returns>No content if the update was successful.</returns>
//         /// <response code="204">Book was successfully updated.</response>
//         /// <response code="400">Invalid book data was provided.</response>
//         /// <response code="404">Book with the specified id was not found.</response>
//         [HttpPut("{id:int}")]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public IActionResult PutUpdateById(int id, [FromBody] CreateBookRequest book)
//         {
//             if (book == null)
//                 return BadRequest("Book data must be provided.");

//             var target = _books.FirstOrDefault(b => b.Id == id);

//             if (target == null)
//                 return NotFound("Book not found.");

//             _mapper.Map(book, target);

//             return NoContent();
//         }

//         /// <summary>
//         /// Partially updates an existing book by its identifier.
//         /// </summary>
//         /// <param name="id">The identifier of the book to update.</param>
//         /// <param name="book">The updated book data.</param>
//         /// <returns>No content if the update was successful.</returns>
//         /// <response code="204">Book was successfully updated.</response>
//         /// <response code="400">Invalid book data was provided.</response>
//         /// <response code="404">Book with the specified id was not found.</response>
//         [HttpPatch("{id:int}")]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public IActionResult PatchUpdateById(int id, [FromBody] UpdateBookRequest book)
//         {
//             if (book == null)
//                 return BadRequest("Book data must be provided.");

//             var target = _books.FirstOrDefault(b => b.Id == id);

//             if (target == null)
//                 return NotFound("Book not found.");

//             _mapper.Map(book, target);

//             return NoContent();
//         }

//         /// <summary>
//         /// Deletes a book by its identifier.
//         /// </summary>
//         /// <param name="id">The identifier of the book to delete.</param>
//         /// <returns>No content if the deletion was successful.</returns>
//         /// <response code="204">Book was successfully deleted.</response>
//         /// <response code="404">Book with the specified id was not found.</response>
//         [HttpDelete("{id:int}")]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public IActionResult DeleteById(int id)
//         {
//             var target = _books.FirstOrDefault(b => b.Id == id);

//             if (target == null)
//                 return NotFound("Book not found.");

//             _books.Remove(target);

//             return NoContent();
//         }
//     }
// }