using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;

namespace hw_2_2_3_26.Services;

public interface IGenreService
{
    Task<IEnumerable<GenreSummaryDto>> GetAllGenres(CancellationToken ct);
    Task<GenreDetailDto?> GetGenreById(int id, CancellationToken ct);
    Task<IEnumerable<GenreDetailDto>> GetGenreBySearchParameters(GenreSearchParameters parameters, CancellationToken ct);
    Task<GenreSummaryDto> Create(CreateGenreRequest request, CancellationToken ct);
    Task<bool> Update(int id, UpdateGenreRequest request, CancellationToken ct);
    Task<bool> PartialUpdate(int id, PartialUpdateGenreRequest request, CancellationToken ct);
    Task<bool> Delete(int id, CancellationToken ct);
}