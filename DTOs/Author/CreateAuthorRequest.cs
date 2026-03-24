using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record CreateAuthorRequest
(
    [Required(ErrorMessage = "Author's first name is required!")]
    [MinLength(1, ErrorMessage = "Author creds can not be shorter than 1 characters long!")]
    string FirstName,

    [Required(ErrorMessage = "Author's last name is required!")]
    [MinLength(1, ErrorMessage = "Author creds can not be shorter than 1 characters long!")]
    string LastName,

    DateTime DateOfBirth,

    int? CountryId,

    ICollection<int> BookIds
);