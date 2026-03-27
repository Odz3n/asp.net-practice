using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public class PublisherDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public CountrySummaryDto? Country { get; set; }
    public DateTime? FoundedAt { get; set; }
    public ICollection<BookSummaryDto> Books { get; set; } = new List<BookSummaryDto>();
}