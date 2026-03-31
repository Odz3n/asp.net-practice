using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using hw_2_2_3_26.Models;
using hw_2_2_3_26.Repository;

namespace hw_2_2_3_26.Services;

public class BookService : IBookService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    private readonly FileService _fileService;
    private readonly IBookRepository _bookRepository;
    public BookService(IMapper mapper, AppDbContext db, FileService fileService, IBookRepository bookRepository)
    {
        _mapper = mapper;
        _db = db;
        _fileService = fileService;
        _bookRepository = bookRepository;
    }
    public async Task<BookDetailDto> Create(CreateBookRequest request, CancellationToken ct)
    {
        var newBook = _mapper.Map<Book>(request);

        // auto-map of book Authors and Genres is ignored in profile
        // and handled there manually
        newBook.BookAuthors = request.AuthorIds
            .Select(id => new BookAuthor { AuthorId = id })
            .ToList();

        newBook.BookGenres = request.GenreIds
            .Select(id => new BookGenre { GenreId = id })
            .ToList();

        await _db.Books.AddAsync(newBook, ct);
        await _db.SaveChangesAsync(ct);

        await UpdateCovers(
            newBook.Covers,
            request.Covers,
            newBook.Id,
            newBook.Title,
            ct);

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<Book, BookDetailDto>(newBook);
    }

    public async Task<bool> Delete(int id, CancellationToken ct)
    {
        var target = await _db.Books
            .Where(b => b.Id == id)
            .Include(b => b.Covers)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Book not found");

        if (target.Covers != null && target.Covers.Any())
            foreach (var cover in target.Covers)
                await _fileService.DeleteFile(cover.Url, ct);

        await _fileService.DeleteEntityDirectoryAsync(
            ContentType.Books,
            target.Id,
            ct);

        _db.Books.Remove(target);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<PagedResult<BookSummaryDto>> GetAllBooks(BookGetParameters parameters, CancellationToken ct)
    {
        var query = _bookRepository.GetBooksAsync(parameters, ct);

        return await query
            .ToPagedResultAsync<Book, BookSummaryDto>(
                parameters,
                _mapper.ConfigurationProvider,
                ct);
    }

    public async Task<BookDetailDto?> GetBookById(int id, CancellationToken ct)
    {
        var book = await _bookRepository.GetUntrackedBookByIdAsync(id, ct);
        if (book == null)
            throw new KeyNotFoundException("Book not found");
        return _mapper.Map<BookDetailDto>(book);
    }

    public async Task<IEnumerable<BookDetailDto>> GetBooksByTitleAndAuthor(BookSearchParameters parameters, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(parameters.Title))
            throw new ArgumentException("Title is required");

        var query = _db.Books
            .AsNoTracking()
            .Where(b => b.Title.ToLowerInvariant().Contains(parameters.Title.ToLowerInvariant()));

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

    public async Task<bool> PartialUpdate(int id, UpdateBookRequest request, CancellationToken ct)
    {
        var target = await _db.Books
            .Where(el => el.Id == id)
            .Include(el => el.BookAuthors)
            .Include(el => el.BookGenres)
            .Include(el => el.Covers)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Book not found");


        _mapper.Map(request, target);

        if (request.AuthorIds != null)
        {
            UpdateCollection(
                target.BookAuthors,
                request.AuthorIds,
                ba => ba.AuthorId,
                id => new BookAuthor { AuthorId = id }
            );
        }

        if (request.GenreIds != null)
        {
            UpdateCollection(
                target.BookGenres,
                request.GenreIds,
                bg => bg.GenreId,
                id => new BookGenre { GenreId = id }
            );
        }

        if (request.Covers != null)
            await UpdateCovers(
                target.Covers,
                request.Covers,
                target.Id,
                target.Title,
                ct);

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Update(int id, CreateBookRequest request, CancellationToken ct)
    {
        var target = await _db.Books
            .Where(b => b.Id == id)
            .Include(b => b.BookAuthors)
            .Include(b => b.BookGenres)
            .Include(b => b.Covers)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Book not found");

        _mapper.Map(request, target);

        UpdateCollection(
            target.BookAuthors,
            request.AuthorIds,
            ba => ba.AuthorId,
            id => new BookAuthor { AuthorId = id }
        );

        UpdateCollection(
            target.BookGenres,
            request.GenreIds,
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
    public async Task<bool> BookExists(int id, CancellationToken ct)
    {
        return await _db.Books.AnyAsync(el => el.Id == id, ct);
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