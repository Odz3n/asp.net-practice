using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}