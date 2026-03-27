namespace hw_2_2_3_26.Models;

public class Cover
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Url { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
    public Book Book { get; set; } = null!;
}