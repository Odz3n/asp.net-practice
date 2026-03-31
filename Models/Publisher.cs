using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.Models;

public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public DateTime? FoundedAt { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
}