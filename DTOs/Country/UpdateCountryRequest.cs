using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record UpdateCountryRequest(
    string Name,
    IEnumerable<int> AuthorIds,
    IEnumerable<int> PublisherIds
);