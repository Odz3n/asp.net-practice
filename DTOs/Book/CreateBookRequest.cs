namespace hw_2_2_3_26.DTO;
public record CreateBookRequest
(
    string Title,
    int Year,
    int PageCount,
    int? PublisherId,
    DateTime CreatedAt,
    ICollection<int>? AuthorIds,
    ICollection<int>? GenreIds,
    ICollection<IFormFile>? Covers
);