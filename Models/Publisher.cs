using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.Models;

public class Publisher
{
    public int Id { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Publisher's name can not be shorter than 1 characters long!")]
    public string Name { get; set; } = null!;

    public int? CountryId { get; set; }
    public Country? Country { get; set; }

    public DateTime? FoundedAt { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}