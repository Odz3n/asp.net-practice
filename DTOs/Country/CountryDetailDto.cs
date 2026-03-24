using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public class CountryDetailDto()
{
    public int Id { get; set; }

    [MinLength(1, ErrorMessage = "Country's name can not be shorter than 1 characters long!")]
    public string Name { get; set; } = null!;

    public ICollection<AuthorSummaryDto> Authors { get; set; } = new List<AuthorSummaryDto>();
    
    public ICollection<PublisherSummaryDto> Publishers { get; set; } = new List<PublisherSummaryDto>();
}