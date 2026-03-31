using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record UpdatePublisherRequest(
    string? Name,
    int? CountryId,
    DateTime FoundedAt,
    IEnumerable<int>? BookIds
);