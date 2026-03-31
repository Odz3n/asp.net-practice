using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record CreateCountryRequest(
    string Name = null!,
    IEnumerable<int>? AuthorIds = null,
    IEnumerable<int>? PublisherIds = null
);