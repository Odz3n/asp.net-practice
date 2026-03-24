using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;

namespace hw_2_2_3_26.Services;

public class BookService : IBookService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public BookService(IMapper mapper, AppDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }
    public async Task<BookDetailDto> Create(CreateBookRequest request, CancellationToken ct)
    {
        var newBook = _mapper.Map<Book>(request);

        // Author and Genre ids Validations
        var validAuthorIds = await _db.Authors
            .Where(a => request.AuthorIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(ct);

        var validGenreIds = await _db.Genres
            .Where(a => request.GenreIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(ct);

        // auto-map of book Authors and Genres is ignored in profile
        // and handled there manually
        newBook.BookAuthors = validAuthorIds
            .Select(id => new BookAuthor { AuthorId = id })
            .ToList();

        newBook.BookGenres = validGenreIds
            .Select(id => new BookGenre { GenreId = id })
            .ToList();

        await _db.Books.AddAsync(newBook, ct);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<Book, BookDetailDto>(newBook);
    }

    public async Task<bool> Delete(int id, CancellationToken ct)
    {
        var target = await _db.Books
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            return false;

        _db.Books.Remove(target);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<IEnumerable<BookSummaryDto>> GetAllBooks(CancellationToken ct)
    {
        var query = await _db.Books
            .AsNoTracking()
            .ToListAsync(ct);
        var books = _mapper.Map<IEnumerable<BookSummaryDto>>(query);
        return books;
    }

    public async Task<BookDetailDto?> GetBookById(int id, CancellationToken ct)
    {
        var target = await _db.Books
            .Where(e => e.Id == id)
            .ProjectTo<BookDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return target;
    }

    public async Task<IEnumerable<BookDetailDto>> GetBooksByTitleAndAuthor(BookSearchParameters parameters, CancellationToken ct)
    {
        var query = _db.Books
            .AsNoTracking()
            .Where(b => b.Title.ToLower().Contains(parameters.Title));

        if (!string.IsNullOrWhiteSpace(parameters.Author))
        {
            query = query.Where(b =>
                b.BookAuthors.Any(ba =>
                    (ba.Author.FirstName + " " + ba.Author.LastName)
                    .Contains(parameters.Author)));
        }

        var target = await query
            .ProjectTo<BookDetailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return target;
    }

    public async Task<bool> PartialUpdate(int id, UpdateBookRequest request, CancellationToken ct)
    {
        var target = await _db.Books
            .Include(el => el.BookAuthors)
            .Include(el => el.BookGenres)
            .FirstOrDefaultAsync(el => el.Id == id, ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        if (request.AuthorIds != null)
        {
            var validAuthorIds = await _db.Authors
                .Where(a => request.AuthorIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync(ct);

            target.BookAuthors
                .Where(ba => !validAuthorIds.Contains(ba.AuthorId))
                .ToList()
                .ForEach(ba => target.BookAuthors.Remove(ba));

            foreach (var idToAdd in validAuthorIds.Except(target.BookAuthors.Select(ba => ba.AuthorId)))
                target.BookAuthors.Add(new BookAuthor { AuthorId = idToAdd });
        }

        if (request.GenreIds != null)
        {
            var validGenreIds = await _db.Genres
                .Where(g => request.GenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync(ct);

            target.BookGenres
                .Where(bg => !validGenreIds.Contains(bg.GenreId))
                .ToList()
                .ForEach(bg => target.BookGenres.Remove(bg));

            foreach (var idToAdd in validGenreIds.Except(target.BookGenres.Select(bg => bg.GenreId)))
                target.BookGenres.Add(new BookGenre { GenreId = idToAdd });
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Update(int id, CreateBookRequest request, CancellationToken ct)
    {
        var target = await _db.Books
            .Include(b => b.BookAuthors)
            .Include(b => b.BookGenres)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        // Validate IDs
        var validAuthorIds = await _db.Authors
            .Where(a => request.AuthorIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(ct);

        var validGenreIds = await _db.Genres
            .Where(g => request.GenreIds.Contains(g.Id))
            .Select(g => g.Id)
            .ToListAsync(ct);

        // Update authors
        target.BookAuthors
            .Where(ba => !validAuthorIds.Contains(ba.AuthorId))
            .ToList()
            .ForEach(ba => target.BookAuthors.Remove(ba));

        foreach (var idToAdd in validAuthorIds.Except(target.BookAuthors.Select(ba => ba.AuthorId)))
            target.BookAuthors.Add(new BookAuthor { AuthorId = idToAdd });

        // Update genres
        target.BookGenres
            .Where(bg => !validGenreIds.Contains(bg.GenreId))
            .ToList()
            .ForEach(bg => target.BookGenres.Remove(bg));

        foreach (var idToAdd in validGenreIds.Except(target.BookGenres.Select(bg => bg.GenreId)))
            target.BookGenres.Add(new BookGenre { GenreId = idToAdd });

        await _db.SaveChangesAsync(ct);
        return true;
    }
}