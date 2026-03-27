using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using hw_2_2_3_26.Models;

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

        var validBookIds = await _db.Books
            .Where(el => request.BookIds.Contains(el.Id))
            .Select(el => el.Id)
            .ToListAsync(ct);

        newGenre.BookGenres = validBookIds
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
            return false;

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
        return await _db.Genres
            .AsNoTracking()
            .Where(el => el.Id == id)
            .ProjectTo<GenreDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
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
            return false;

        _mapper.Map(request, target);

        if (request.BookIds != null)
        {
            var validBookIds = await _db.Books
                .Where(el => request.BookIds.Contains(el.Id))
                .Select(el => el.Id)
                .ToListAsync(ct);

            target.BookGenres
                .Where(el => !validBookIds.Contains(el.BookId))
                .ToList()
                .ForEach(el => target.BookGenres.Remove(el));

            foreach (var idToAdd in validBookIds.Except(target.BookGenres.Select(el => el.BookId)))
                target.BookGenres.Add(new BookGenre { BookId = idToAdd });
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
            return false;

        _mapper.Map(request, target);

        var validBookIds = await _db.Books
            .Where(el => request.BookIds.Contains(el.Id))
            .Select(el => el.Id)
            .ToListAsync(ct);

        target.BookGenres
            .Where(el => !validBookIds.Contains(el.BookId))
            .ToList()
            .ForEach(el => target.BookGenres.Remove(el));

        foreach (var idToAdd in validBookIds.Except(target.BookGenres.Select(el => el.BookId)))
            target.BookGenres.Add(new BookGenre { BookId = idToAdd });

        await _db.SaveChangesAsync(ct);
        return true;
    }
}