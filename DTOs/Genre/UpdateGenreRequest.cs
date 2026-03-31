using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record UpdateGenreRequest
(
    string? Name,
    IEnumerable<int>? BookIds
);