using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record UpdateAuthorRequest
(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    int? CountryId,
    ICollection<int>? BookIds
);