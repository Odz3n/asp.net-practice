using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Models;
using hw_2_2_3_26.Services;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;

namespace hw_2_2_3_26.Repository;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _db;
    public BookRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task AddBookAsync(Book book, CancellationToken ct)
    {
        await _db.AddAsync(book, ct);
    }

    public async Task<Book?> GetUntrackedBookByIdAsync(int? id, CancellationToken ct)
    {
        return await _db.Books
            .AsNoTracking()
            .Where(el => el.Id == id)
            .Include(el => el.Publisher)
            .Include(el => el.Covers)
            .Include(el => el.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(el => el.BookGenres)
                .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(el => el.Id == id, ct);
    }

    public async Task<Book?> GetTrackedBookByIdAsync(int? id, CancellationToken ct)
    {
        return await _db.Books
            .Where(el => el.Id == id)
            .Include(el => el.Publisher)
            .Include(el => el.Covers)
            .Include(el => el.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(el => el.BookGenres)
                .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(el => el.Id == id, ct);
    }

    public IQueryable<Book> GetBooksAsync(BookGetParameters parameters, CancellationToken ct)
    {
        return _db.Books
            .AsNoTracking()
            .ApplyFilters(parameters)
            .ApplySorting(parameters);
    }

    public Task RemoveBook(Book book, CancellationToken ct)
    {
        _db.Books.Remove(book);
        return Task.CompletedTask;
    }

    public Task RemoveBookAuthors(ICollection<BookAuthor> bookAuthors)
    {
        _db.RemoveRange(bookAuthors);
        return Task.CompletedTask;
    }

    public Task RemoveBookGenres(ICollection<BookGenre> bookGenres)
    {
        _db.RemoveRange(bookGenres);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
    public void UpdateBookAuthors(Book book, IEnumerable<int>? authorIds)
    {
        UpdateCollection(
            book.BookAuthors,
            authorIds,
            el => el.AuthorId,
            id => new BookAuthor { AuthorId = id });
    }
    public void UpdateBookGenres(Book book, IEnumerable<int>? genreIds)
    {
        UpdateCollection(
            book.BookGenres,
            genreIds,
            el => el.GenreId,
            id => new BookGenre { GenreId = id });
    }
    private void UpdateCollection<TEntity, TValue>(
        ICollection<TEntity> collection,
        IEnumerable<TValue>? validIds,
        Func<TEntity, TValue> getId,
        Func<TValue, TEntity> createEntity
    ) where TEntity : class
    {
        var validIdsList = validIds.ToList();
        var toRemove = collection
            .Where(item => !validIdsList.Contains(getId(item)))
            .ToList();

        foreach (var item in toRemove)
            collection.Remove(item);

        var existingIds = collection.Select(getId).ToList();
        var idsToAdd = validIdsList.Except(existingIds).ToList();

        foreach (var id in idsToAdd)
            collection.Add(createEntity(id));
    }

    public async Task<bool> BookExistsAsync(int? id, CancellationToken ct)
    {
        return await _db.Books.AnyAsync(el => el.Id == id, ct);
    }

    public IQueryable<Book> GetUntrackedBooksBySearchParameters(BookSearchParameters parameters)
    {
        var query = _db.Books
            .AsNoTracking()
            .Where(b => b.Title.ToLowerInvariant()
                .Contains(parameters.Title!.ToLowerInvariant()));

        if (!string.IsNullOrWhiteSpace(parameters.Author))
        {
            query = query.Where(b =>
                b.BookAuthors.Any(ba =>
                    (ba.Author.FirstName.ToLowerInvariant() + " " + ba.Author.LastName.ToLowerInvariant())
                    .Contains(parameters.Author.ToLowerInvariant())));
        }
        return query;
    }
}