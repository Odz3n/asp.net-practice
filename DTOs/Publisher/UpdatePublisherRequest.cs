using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record UpdatePublisherRequest(
    [MinLength(1, ErrorMessage = "Publisher's name can not be shorter than 1 characters long!")]
    string Name,
    int CountryId,
    DateTime FoundedAt,
    IEnumerable<int> BookIds
);