using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Models;
using hw_2_2_3_26.Repository;

namespace hw_2_2_3_26.Services;

public class BookService : IBookService
{
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IBookRepository _bookRepository;
    public BookService(IMapper mapper, IFileService fileService, IBookRepository bookRepository)
    {
        _mapper = mapper;
        _fileService = fileService;
        _bookRepository = bookRepository;
    }
    public async Task<BookDetailDto> Create(CreateBookRequest request, CancellationToken ct)
    {
        var newBook = _mapper.Map<Book>(request);

        if (request.AuthorIds != null)
            _bookRepository.UpdateBookAuthors(newBook, request.AuthorIds);
        if (request.GenreIds != null)
            _bookRepository.UpdateBookGenres(newBook, request.GenreIds);

        await _bookRepository.AddBookAsync(newBook, ct);
        // Save beforehead to get ID early
        await _bookRepository.SaveChangesAsync(ct);

        if (request.Covers != null)
            await UpdateBookCovers(newBook, request.Covers, ct);

        await _bookRepository.SaveChangesAsync(ct);
        return _mapper.Map<Book, BookDetailDto>(newBook);
    }

    public async Task<bool> Delete(int? id, CancellationToken ct)
    {
        var target = await _bookRepository.GetTrackedBookByIdAsync(id, ct);

        if (target == null)
            throw new KeyNotFoundException("Book not found");

        if (target.Covers != null && target.Covers.Any())
            foreach (var cover in target.Covers)
                await _fileService.DeleteFile(cover.Url, ct);

        await _fileService.DeleteEntityDirectoryAsync(
            ContentType.Books,
            target.Id,
            ct);

        await _bookRepository.RemoveBook(target, ct);
        await _bookRepository.SaveChangesAsync(ct);

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
        var query = _bookRepository.GetUntrackedBooksBySearchParameters(parameters);
        return query.ProjectTo<BookDetailDto>(_mapper.ConfigurationProvider);
    }

    public async Task<bool> PartialUpdate(int? id, UpdateBookRequest request, CancellationToken ct)
    {
        var target = await _bookRepository.GetTrackedBookByIdAsync(id, ct);

        if (target == null)
            throw new KeyNotFoundException("Book not found");

        _mapper.Map(request, target);

        if (request.AuthorIds != null)
            _bookRepository.UpdateBookAuthors(target, request.AuthorIds);
        if (request.GenreIds != null)
            _bookRepository.UpdateBookGenres(target, request.GenreIds);
        if (request.Covers != null)
            await UpdateBookCovers(target, request.Covers, ct);

        await _bookRepository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Update(int? id, CreateBookRequest request, CancellationToken ct)
    {
        var target = await _bookRepository.GetTrackedBookByIdAsync(id, ct);

        if (target == null)
            throw new KeyNotFoundException("Book not found");

        _mapper.Map(request, target);

        if (request.AuthorIds != null)
            _bookRepository.UpdateBookAuthors(target, request.AuthorIds);
        if (request.GenreIds != null)
            _bookRepository.UpdateBookGenres(target, request.GenreIds);
        if (request.Covers != null)
            await UpdateBookCovers(target, request.Covers, ct);

        await _bookRepository.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> BookExists(int? id, CancellationToken ct)
    {
        return await _bookRepository.BookExistsAsync(id, ct);
    }
    private async Task UpdateBookCovers(Book book, IEnumerable<IFormFile> covers, CancellationToken ct)
    {
        await UpdateCovers(
            book.Covers,
            covers,
            book.Id,
            ct);
    }
    private async Task UpdateCovers(
        ICollection<Cover> collection,
        IEnumerable<IFormFile>? files,
        int entityId,
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