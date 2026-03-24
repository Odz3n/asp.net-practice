namespace hw_2_2_3_26.DTO;

using System.ComponentModel.DataAnnotations;
using MyApp.Validators;

public record UpdateBookRequest
(
    [MinLength(1, ErrorMessage = "Title can not be shorter than 1 characters long!")]
    string? Title,

    [ValidateYear(1800, 2026)]
    int? Year,

    [Range(1, 10000)]
    int? PageCount,

    int? PublisherId,

    ICollection<int>? AuthorIds,
    ICollection<int>? GenreIds
);