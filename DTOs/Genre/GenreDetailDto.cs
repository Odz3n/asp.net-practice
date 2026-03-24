namespace hw_2_2_3_26.DTO;

public class GenreDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<BookSummaryDto> Books {get; set;} = new List<BookSummaryDto>();
}