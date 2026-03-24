using System.ComponentModel.DataAnnotations;

namespace hw_2_2_3_26.DTO;

public record CreateGenreRequest
(
    [MinLength(1, ErrorMessage = "Genre name can not be shorter than 1 characters long!")]
    string Name,
    
    IEnumerable<int> BookIds
);