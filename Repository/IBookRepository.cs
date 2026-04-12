using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Repository;

public interface IBookRepository
{
    Task AddBookAsync(Book book, CancellationToken ct);
    Task<bool> BookExistsAsync(int? id, CancellationToken ct);
    IQueryable<Book> GetBooksAsync(BookGetParameters parameters, CancellationToken ct);
    Task<Book?> GetUntrackedBookByIdAsync(int? id, CancellationToken ct);
    IQueryable<Book> GetUntrackedBooksBySearchParameters(BookSearchParameters parameters);
    Task<Book?> GetTrackedBookByIdAsync(int? id, CancellationToken ct);
    Task RemoveBook(Book book, CancellationToken ct);
    Task RemoveBookAuthors(ICollection<BookAuthor> bookAuthors);
    Task RemoveBookGenres(ICollection<BookGenre> bookGenres);
    Task SaveChangesAsync(CancellationToken ct);
    void UpdateBookAuthors(Book book, IEnumerable<int>? authorIds);
    void UpdateBookGenres(Book book, IEnumerable<int>? genreIds);
}