// using Asp.Versioning;
// using Microsoft.AspNetCore.Mvc;
// using MyApp.Models;
// using Swashbuckle.AspNetCore.Annotations;

// namespace MyApp.Controllers.v1
// {
//     [ApiVersion("1.0")]
//     [Route("api/v{version:apiVersion}/[controller]")]
//     [ApiController]
//     [Produces("application/json")]
//     [SwaggerTag("Provides API endpoints for managing books.")]
//     public class BooksController : ControllerBase
//     {
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

//         [HttpGet]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         public ActionResult<IEnumerable<Book>> GetBooks()
//         {
//             return Ok(_books);
//         }

//         [HttpGet("{id:int}")]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public ActionResult<Book> GetById(int id)
//         {
//             var target = _books.FirstOrDefault(b => b.Id == id);

//             if (target == null)
//                 return NotFound("Book not found.");

//             return Ok(target);
//         }

//         [HttpGet("search")]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         public ActionResult<IEnumerable<Book>> GetByTitleAndAuthor([FromQuery] string? title, string? author)
//         {
//             if (string.IsNullOrWhiteSpace(title))
//                 return BadRequest("Search target title must be provided.");

//             var query = _books.Where(b =>
//                 b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

//             if (!string.IsNullOrWhiteSpace(author))
//             {
//                 var books = query.Where(b =>
//                     b.AuthorFirstName.Contains(author, StringComparison.OrdinalIgnoreCase) ||
//                     b.AuthorLastName.Contains(author, StringComparison.OrdinalIgnoreCase));

//                 return Ok(books);
//             }

//             return Ok(query);
//         }

//         [HttpPost]
//         [ProducesResponseType(StatusCodes.Status201Created)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         public ActionResult<Book> Create([FromBody] Book book)
//         {
//             if (book == null)
//                 return BadRequest("Book's data must be provided.");

//             var newBook = new Book
//             {
//                 Id = _books.Max(b => b.Id) + 1,
//                 Title = book.Title,
//                 AuthorFirstName = book.AuthorFirstName,
//                 AuthorLastName = book.AuthorLastName,
//                 Year = book.Year
//             };

//             _books.Add(newBook);

//             return CreatedAtAction(nameof(GetById), new { Id = newBook.Id }, newBook);
//         }

//         [HttpPut("{id:int}")]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public IActionResult UpdateById(int id, [FromBody] Book book)
//         {
//             if (book == null)
//                 return BadRequest("Book data must be provided.");

//             var target = _books.FirstOrDefault(b => b.Id == id);

//             if (target == null)
//                 return NotFound("Book not found.");

//             target.Title = book.Title;
//             target.AuthorFirstName = book.AuthorFirstName;
//             target.AuthorLastName = book.AuthorLastName;
//             target.Year = book.Year;

//             return NoContent();
//         }

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