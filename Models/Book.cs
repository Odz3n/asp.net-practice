using System.ComponentModel.DataAnnotations;
using MyApp.Validators;

namespace MyApp.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required!")]
    [MinLength(1, ErrorMessage = "Title can not be shorter than 1 characters long!")]
    public string Title { get; set; } = null!;
    
    [Required]
    [ValidateYear(1800, 2026)]
    public int Year { get; set; }
    
    [Range(1, 10000)]
    public int PageCount { get; set; }

    public int? PublisherId { get; set; }
    public Publisher? Publisher { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}
