namespace hw_2_2_3_26.DTO;

using System.ComponentModel.DataAnnotations;
public record UpdateBookRequest
(
    string? Title,
    int? Year,
    int? PageCount,
    int? PublisherId,
    ICollection<int>? AuthorIds,
    ICollection<int>? GenreIds,
    ICollection<IFormFile>? Covers
);