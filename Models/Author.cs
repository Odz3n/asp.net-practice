using System.ComponentModel.DataAnnotations;
using MyApp.Validators;

namespace hw_2_2_3_26.Models;

public class Author
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Author's first name is required!")]
    [MinLength(1, ErrorMessage = "Author creds can not be shorter than 1 characters long!")]
    public string FirstName { get; set; } = null!;
    
    [Required(ErrorMessage = "Author's last name is required!")]
    [MinLength(1, ErrorMessage = "Author creds can not be shorter than 1 characters long!")]
    public string LastName { get; set; } = null!;
    
    public DateTime DateOfBirth { get; set; }
    
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}