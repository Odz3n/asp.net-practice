using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record PartialUpdateAuthorRequest
(
    [MinLength(1, ErrorMessage = "Author creds can not be shorter than 1 characters long!")]
    string? FirstName,
    
    [MinLength(1, ErrorMessage = "Author creds can not be shorter than 1 characters long!")]
    string? LastName,

    DateTime? DateOfBirth,

    int? CountryId,

    ICollection<int>? BookIds
);