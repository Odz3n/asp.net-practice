using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using hw_2_2_3_26.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace hw_2_2_3_26.Services;

public class GenreService : IGenreService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public GenreService(IMapper mapper, AppDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }
    public async Task<GenreSummaryDto> Create(CreateGenreRequest request, CancellationToken ct)
    {
        var newGenre = _mapper.Map<Genre>(request);

        if (request.BookIds != null)
            newGenre.BookGenres = request.BookIds
                .Select(id => new BookGenre { BookId = id })
                .ToList();

        await _db.Genres.AddAsync(newGenre, ct);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<GenreSummaryDto>(newGenre);
    }

    public async Task<bool> Delete(int id, CancellationToken ct)
    {
        var target = await _db.Genres
            .FirstOrDefaultAsync(el => el.Id == id, ct);

        if (target == null)
            throw new KeyNotFoundException("Genre not found");

        _db.Genres.Remove(target);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<IEnumerable<GenreSummaryDto>> GetAllGenres(CancellationToken ct)
    {
        return await _db.Genres
            .AsNoTracking()
            .ProjectTo<GenreSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<GenreDetailDto?> GetGenreById(int id, CancellationToken ct)
    {
        var target = await _db.Genres
            .AsNoTracking()
            .Where(el => el.Id == id)
            .ProjectTo<GenreDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Genre not found");

        return target;
    }

    public async Task<IEnumerable<GenreDetailDto>> GetGenreBySearchParameters(GenreSearchParameters parameters, CancellationToken ct)
    {
        var query = _db.Genres
            .AsNoTracking();

        if (parameters.Name != null)
        {
            query = query
                .Where(el => EF.Functions.Like(el.Name, $"%{parameters.Name}%"));
        }

        return await query
            .ProjectTo<GenreDetailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<bool> PartialUpdate(int id, PartialUpdateGenreRequest request, CancellationToken ct)
    {
        var target = await _db.Genres
            .Where(el => el.Id == id)
            .Include(el => el.BookGenres)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Genre not found");

        _mapper.Map(request, target);

        if (request.BookIds != null)
        {
            UpdateCollection(
                target.BookGenres,
                request.BookIds,
                bg => bg.BookId,
                id => new BookGenre { BookId = id });
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Update(int id, UpdateGenreRequest request, CancellationToken ct)
    {
        var target = await _db.Genres
            .Where(el => el.Id == id)
            .Include(el => el.BookGenres)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            throw new KeyNotFoundException("Genre not found");

        _mapper.Map(request, target);

        if (request.BookIds != null)
        {
            UpdateCollection(
                target.BookGenres,
                request.BookIds,
                bg => bg.BookId,
                id => new BookGenre { BookId = id });
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> GenreExists(int id, CancellationToken ct)
    {
        return await _db.Genres.AnyAsync(el => el.Id == id, ct);
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