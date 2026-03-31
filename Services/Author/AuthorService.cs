using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using hw_2_2_3_26.Services;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using hw_2_2_3_26.Models;
using Microsoft.AspNetCore.Mvc;

public class AuthorService : IAuthorService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public AuthorService(IMapper mapper, AppDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }
    public async Task<AuthorSummaryDto> Create(CreateAuthorRequest request, CancellationToken ct)
    {
        var newAuthor = _mapper.Map<Author>(request);
        
        if (request.BookIds != null)
            UpdateCollection(
                newAuthor.BookAuthors,
                request.BookIds,
                el => el.BookId,
                id => new BookAuthor { BookId = id });

        await _db.Authors.AddAsync(newAuthor, ct);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<AuthorSummaryDto>(newAuthor);
    }

    public async Task<bool> Delete(int id, CancellationToken ct)
    {
        var target = await _db.Authors.FirstOrDefaultAsync(el => el.Id == id, ct);
        if (target == null)
            throw new KeyNotFoundException("Author not found");

        _db.Authors.Remove(target);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PagedResult<AuthorSummaryDto>> GetAllAuthors(AuthorGetParameters parameters, CancellationToken ct)
    {
        return await _db.Authors
            .AsNoTracking()
            .ApplyFilters(parameters)
            .ApplySorting(parameters)
            .ToPagedResultAsync<Author, AuthorSummaryDto>(parameters, _mapper.ConfigurationProvider, ct);
    }

    public async Task<AuthorDetailDto?> GetAuthorById(int id, CancellationToken ct)
    {
        var target = await _db.Authors
            .Where(el => el.Id == id)
            .ProjectTo<AuthorDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Author not found");

        return target;
    }

    public async Task<IEnumerable<AuthorDetailDto>> GetAuthorBySearchParameters(AuthorSearchParameters parameters, CancellationToken ct)
    {
        var query = _db.Authors.AsNoTracking();

        if (parameters.LastName != null)
            query = query
                .Where(el => EF.Functions.Like(el.LastName, $"%{parameters.LastName}%"));

        if (parameters.FirstName != null)
            query = query
                .Where(el => EF.Functions.Like(el.FirstName, $"%{parameters.FirstName}%"));

        if (parameters.Country != null)
            query = query
                .Where(el => el.Country != null && EF.Functions.Like(el.Country.Name, $"%{parameters.Country}%"));

        return await query
            .ProjectTo<AuthorDetailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<bool> PartialUpdate(int id, PartialUpdateAuthorRequest request, CancellationToken ct)
    {
        var target = await _db.Authors
            .Where(el => el.Id == id)
            .Include(el => el.BookAuthors)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Author not found");

        _mapper.Map(request, target);

        if (request.BookIds != null)
            UpdateCollection(
                target.BookAuthors,
                request.BookIds,
                el => el.BookId,
                id => new BookAuthor { BookId = id });

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Update(int id, UpdateAuthorRequest request, CancellationToken ct)
    {
        var target = await _db.Authors
            .Where(el => el.Id == id)
            .Include(el => el.BookAuthors)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Author not found");

        _mapper.Map(request, target);

        if (request.BookIds != null)
            UpdateCollection(
                target.BookAuthors,
                request.BookIds,
                el => el.BookId,
                id => new BookAuthor { BookId = id });

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AuthorExists(int authorId, CancellationToken ct)
    {
        return await _db.Authors.AnyAsync(el => el.Id == authorId);
    }

    public async Task<bool> AllAuthorsExist(IEnumerable<int> ids, CancellationToken ct)
    {
        var distinctIds = ids.Distinct().ToList();
        var existingIds = await _db.Authors
            .Where(el => distinctIds.Contains(el.Id))
            .Select(el => el.Id)
            .ToListAsync(ct);

        return existingIds.Count == distinctIds.Count;
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
}