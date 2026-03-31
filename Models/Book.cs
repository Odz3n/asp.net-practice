namespace hw_2_2_3_26.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int Year { get; set; }
    public int PageCount { get; set; }
    public int? PublisherId { get; set; }
    public Publisher? Publisher { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    public ICollection<Cover>? Covers { get; set; } = new List<Cover>();
}
