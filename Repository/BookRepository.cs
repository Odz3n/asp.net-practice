using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Models;
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
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Book?> GetUntrackedBookByIdAsync(int id, CancellationToken ct)
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

    public async Task<Book?> GetTrackedBookByIdAsync(int id, CancellationToken ct)
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
}