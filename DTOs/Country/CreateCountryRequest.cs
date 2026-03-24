using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record CreateCountryRequest(
    [Required]
    [MinLength(1, ErrorMessage = "Country's name can not be shorter than 1 characters long!")]
    string Name,

    IEnumerable<int>? AuthorIds,
    IEnumerable<int>? PublisherIds
);