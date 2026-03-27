using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.Models;

public class Genre
{
    public int Id { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Genre name can not be shorter than 1 characters long!")]
    public string Name { get; set; } = null!;
    
    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}