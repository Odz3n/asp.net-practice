using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}