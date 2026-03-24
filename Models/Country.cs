using System.ComponentModel.DataAnnotations;

namespace MyApp.Models;

public class Country
{
    public int Id { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Country's name can not be shorter than 1 characters long!")]
    public string Name { get; set; } = null!;

    public ICollection<Author> Authors { get; set; } = new List<Author>();
    
    public ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
}