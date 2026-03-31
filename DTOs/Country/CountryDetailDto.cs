using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public class CountryDetailDto()
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<AuthorSummaryDto> Authors { get; set; } = new List<AuthorSummaryDto>();
    public ICollection<PublisherSummaryDto> Publishers { get; set; } = new List<PublisherSummaryDto>();
}