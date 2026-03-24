using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;

namespace hw_2_2_3_26.Services;

public interface IAuthorService
{
    Task<PagedResult<AuthorSummaryDto>> GetAllAuthors(AuthorGetParameters parameters, CancellationToken ct);
    Task<AuthorDetailDto?> GetAuthorById(int id, CancellationToken ct);
    Task<IEnumerable<AuthorDetailDto>> GetAuthorBySearchParameters(AuthorSearchParameters parameters, CancellationToken ct);
    Task<AuthorSummaryDto> Create(CreateAuthorRequest request, CancellationToken ct);
    Task<bool> Update(int id, UpdateAuthorRequest request, CancellationToken ct);
    Task<bool> PartialUpdate(int id, PartialUpdateAuthorRequest request, CancellationToken ct);
    Task<bool> Delete(int id, CancellationToken ct);
}