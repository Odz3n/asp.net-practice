using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Repository;

public interface IBookRepository
{
    IQueryable<Book> GetBooksAsync(BookGetParameters parameters, CancellationToken ct);
    Task<Book?> GetUntrackedBookByIdAsync(int id, CancellationToken ct);
    Task<Book?> GetTrackedBookByIdAsync(int id, CancellationToken ct);
    Task AddBookAsync(Book book, CancellationToken ct);
}