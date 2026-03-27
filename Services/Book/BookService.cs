using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using hw_2_2_3_26.Models;
using System.Linq.Expressions;

namespace hw_2_2_3_26.Services;

public class BookService : IBookService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    private readonly FileService _fileService;
    public BookService(IMapper mapper, AppDbContext db, FileService fileService)
    {
        _mapper = mapper;
        _db = db;
        _fileService = fileService;
    }
    public async Task<BookDetailDto> Create(CreateBookRequest request, CancellationToken ct)
    {
        var newBook = _mapper.Map<Book>(request);

        // Author and Genre ids Validations
        var validAuthorIds = await GetValidAuthorIds(request.AuthorIds, ct);
        var validGenreIds = await GetValidGenreIds(request.GenreIds, ct);

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

        if (request.Covers?.Any() == true)
        {
            var isFirst = true;
            // Saves all the provided covers and store in DB
            foreach (var item in request.Covers)
            {
                var url = await _fileService.SaveFileAsync(
                    ContentType.Books,
                    newBook.Id,
                    newBook.Title,
                    SubContentType.Covers,
                    item,
                    ct);

                if (!string.IsNullOrEmpty(url))
                {
                    newBook.Covers.Add(new Cover
                    {
                        BookId = newBook.Id,
                        Url = url,
                        IsPrimary = isFirst,
                        CreatedAt = DateTime.UtcNow
                    });
                    isFirst = false;
                }
            }
            await _db.SaveChangesAsync(ct);
        }
        return _mapper.Map<Book, BookDetailDto>(newBook);
    }

    public async Task<bool> Delete(int id, CancellationToken ct)
    {
        var target = await _db.Books
            .Where(b => b.Id == id)
            .Include(b => b.Covers)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            return false;

        if (target.Covers != null && target.Covers.Any())
            foreach (var cover in target.Covers)
                await _fileService.DeleteFile(cover.Url, ct);

        await _fileService.DeleteEntityDirectoryAsync(
            ContentType.Books,
            target.Id,
            target.Title,
            ct
        );

        _db.Books.Remove(target);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<PagedResult<BookDetailDto>> GetAllBooks(BookGetParameters parameters, CancellationToken ct)
    {
        return await _db.Books
            .AsNoTracking()
            .ApplyFilters(parameters)
            .ApplySorting(parameters)
            .ToPagedResultAsync<Book, BookDetailDto>(parameters, _mapper.ConfigurationProvider, ct);
    }

    public async Task<BookDetailDto?> GetBookById(int id, CancellationToken ct)
    {
        return await _db.Books
            .Where(e => e.Id == id)
            .ProjectTo<BookDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
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

        return await query
            .ProjectTo<BookDetailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    // TODO: Fix the override of a book’s directory when its name is changed.
    // Make sure that only the directory, file, and BD-stored path to the cover is renamed.
    public async Task<bool> PartialUpdate(int id, UpdateBookRequest request, CancellationToken ct)
    {
        var target = await _db.Books
            .Include(el => el.BookAuthors)
            .Include(el => el.BookGenres)
            .Include(el => el.Covers)
            .FirstOrDefaultAsync(el => el.Id == id, ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        if (request.AuthorIds != null)
        {
            var validAuthorIds = await GetValidAuthorIds(request.AuthorIds, ct);

            UpdateCollection(
                target.BookAuthors,
                validAuthorIds,
                ba => ba.AuthorId,
                id => new BookAuthor { AuthorId = id }
            );
        }

        if (request.GenreIds != null)
        {
            var validGenreIds = await GetValidGenreIds(request.GenreIds, ct);

            UpdateCollection(
                target.BookGenres,
                validGenreIds,
                bg => bg.GenreId,
                id => new BookGenre { GenreId = id }
            );
        }

        if (request.Covers != null)
            // Update covers
            await UpdateCovers(
                target.Covers,
                request.Covers,
                target.Id,
                target.Title,
                ct);

        await _db.SaveChangesAsync(ct);
        return true;
    }

    // TODO: Fix the override of a book’s directory when its name is changed.
    // Make sure that only the directory, file, and BD-stored path to the cover is renamed.
    public async Task<bool> Update(int id, CreateBookRequest request, CancellationToken ct)
    {
        var target = await _db.Books
            .Include(b => b.BookAuthors)
            .Include(b => b.BookGenres)
            .Include(b => b.Covers)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        // Validate IDs
        var validAuthorIds = await GetValidAuthorIds(request.AuthorIds, ct);

        var validGenreIds = await GetValidGenreIds(request.GenreIds, ct);

        // Update authors
        UpdateCollection(
            target.BookAuthors,
            validAuthorIds,
            ba => ba.AuthorId,
            id => new BookAuthor { AuthorId = id }
        );

        UpdateCollection(
            target.BookGenres,
            validGenreIds,
            bg => bg.GenreId,
            id => new BookGenre { GenreId = id }
        );


        await UpdateCovers(
            target.Covers,
            request.Covers,
            target.Id,
            target.Title,
            ct);

        await _db.SaveChangesAsync(ct);
        return true;
    }
    private async Task<IEnumerable<int>> GetValidAuthorIds(IEnumerable<int> ids, CancellationToken ct)
    {
        return await _db.Authors
            .Where(el => ids.Contains(el.Id))
            .Select(el => el.Id)
            .ToListAsync(ct);
    }
    private async Task<IEnumerable<int>> GetValidGenreIds(IEnumerable<int> ids, CancellationToken ct)
    {
        return await _db.Genres
            .Where(el => ids.Contains(el.Id))
            .Select(el => el.Id)
            .ToListAsync(ct);
    }
    private void UpdateCollection<TEntity, TValue>(
        ICollection<TEntity> collection,
        IEnumerable<TValue> validIds,
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

    private async Task UpdateCovers(
        ICollection<Cover> collection,
        IEnumerable<IFormFile>? files,
        int entityId,
        string entityTitle,
        CancellationToken ct)
    {
        foreach (var oldCover in collection)
        {
            await _fileService.DeleteFile(oldCover.Url, ct);
        }

        collection.Clear();

        if (files == null || !files.Any())
            return;

        var isFirst = true;
        foreach (var file in files)
        {
            var url = await _fileService.SaveFileAsync(
                ContentType.Books,
                entityId,
                entityTitle,
                SubContentType.Covers,
                file,
                ct);

            if (!string.IsNullOrEmpty(url))
            {
                collection.Add(new Cover
                {
                    BookId = entityId,
                    Url = url,
                    IsPrimary = isFirst,
                    CreatedAt = DateTime.UtcNow
                });
                isFirst = false;
            }
        }
    }
}