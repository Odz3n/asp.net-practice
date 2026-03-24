namespace hw_2_2_3_26.DTO;

public class AuthorDetailDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public CountrySummaryDto? Country { get; set; }
    public IEnumerable<BookSummaryDto>? Books { get; set; }
};