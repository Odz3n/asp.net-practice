namespace hw_2_2_3_26.Models;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
}