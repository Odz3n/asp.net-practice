using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;

namespace hw_2_2_3_26.Services;

public interface IBookService
{
    Task<PagedResult<BookSummaryDto>> GetAllBooks(BookGetParameters parameters, CancellationToken ct);
    Task<BookDetailDto?> GetBookById(int id, CancellationToken ct);
    Task<IEnumerable<BookDetailDto>> GetBooksByTitleAndAuthor(BookSearchParameters parameters, CancellationToken ct);
    Task<BookDetailDto> Create(CreateBookRequest request, CancellationToken ct);
    Task<bool> Update(int id, CreateBookRequest request, CancellationToken ct);
    Task<bool> PartialUpdate(int id, UpdateBookRequest request, CancellationToken ct);
    Task<bool> Delete(int id, CancellationToken ct);
    Task<bool> BookExists(int id, CancellationToken ct);
}