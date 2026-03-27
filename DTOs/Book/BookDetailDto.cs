namespace hw_2_2_3_26.DTO;

public record BookDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!; 
    public int Year { get; set; }
    public IEnumerable<CoverSummaryDto> Covers {get; set;} = new List<CoverSummaryDto>();
    public IEnumerable<AuthorSummaryDto> Authors { get; set; } = new List<AuthorSummaryDto>();
    public IEnumerable<GenreSummaryDto> Genres { get; set; } = new List<GenreSummaryDto>();
    public PublisherSummaryDto? Publisher { get; set; }
    public int PageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}