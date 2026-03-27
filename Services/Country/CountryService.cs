using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Services;

public class CountryService : ICountryService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public CountryService(IMapper mapper, AppDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }
    public async Task<CountrySummaryDto> Create(CreateCountryRequest request, CancellationToken ct)
    {
        var newCountry = _mapper.Map<Country>(request);

        if (request.AuthorIds != null)
        {
            var validAuthorIds = await GetValidAuthorIds(request.AuthorIds, ct);
            await SyncCountryAuthors(newCountry, validAuthorIds, ct);
        }

        if (request.PublisherIds != null)
        {
            var validPublisherIds = await GetValidPublisherIds(request.PublisherIds, ct);
            await SyncCountryPublishers(newCountry, validPublisherIds, ct);
        }

        await _db.Countries.AddAsync(newCountry, ct);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<CountrySummaryDto>(newCountry);
    }

    public async Task<bool> Delete(int id, CancellationToken ct)
    {
        var target = await _db.Countries
            .FirstOrDefaultAsync(el => el.Id == id, ct);
        
        if (target == null)
            return false;

        _db.Countries.Remove(target);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<CountrySummaryDto>> GetAllCountries(CancellationToken ct)
    {
        return await _db.Countries
            .AsNoTracking()
            .ProjectTo<CountrySummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<CountryDetailDto?> GetCountryById(int id, CancellationToken ct)
    {
        return await _db.Countries
            .AsNoTracking()
            .Where(el => el.Id == id)
            .ProjectTo<CountryDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<CountryDetailDto>> GetCountryBySearchParameters(CountrySearchParameters parameters, CancellationToken ct)
    {
        var query = _db.Countries.AsNoTracking();

        if (parameters.Name != null)
            query = query
                .Where(el => EF.Functions.Like(el.Name, $"%{parameters.Name}%"));
        
        return await query
            .ProjectTo<CountryDetailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<bool> PartialUpdate(int id, PartialUpdateCountryRequest request, CancellationToken ct)
    {
        var target = await _db.Countries
            .Where(el => el.Id == id)
            .Include(el => el.Authors)
            .Include(el => el.Publishers)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        if (request.AuthorIds != null)
        {
            var validAuthorIds = await GetValidAuthorIds(request.AuthorIds, ct);
            await SyncCountryAuthors(target, validAuthorIds, ct);
        }

        if (request.PublisherIds != null)
        {
            var validPublisherIds = await GetValidPublisherIds(request.PublisherIds, ct);
            await SyncCountryPublishers(target, validPublisherIds, ct);
        }
        
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Update(int id, UpdateCountryRequest request, CancellationToken ct)
    {
        var target = await _db.Countries
            .Where(el => el.Id == id)
            .Include(el => el.Authors)
            .Include(el => el.Publishers)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        var validAuthorIds = await GetValidAuthorIds(request.AuthorIds, ct);
        await SyncCountryAuthors(target, validAuthorIds, ct);
        var validPublisherIds = await GetValidPublisherIds(request.PublisherIds, ct);
        await SyncCountryPublishers(target, validPublisherIds, ct);
        
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
    private async Task<IEnumerable<int>> GetValidPublisherIds(IEnumerable<int> ids, CancellationToken ct)
    {
        return await _db.Publishers
            .Where(el => ids.Contains(el.Id))
            .Select(el => el.Id)
            .ToListAsync(ct);
    }
    private async Task SyncCountryAuthors(Country country, IEnumerable<int> validIds, CancellationToken ct)
    {
        var currentIds = country.Authors.Select(el => el.Id).ToList();

        var toRemove = country.Authors
            .Where(el => !validIds.Contains(el.Id))
            .ToList();

        foreach(var item in toRemove)
            country.Authors.Remove(item);

        var toAddIds = validIds.Except(currentIds);

        var authorsToAdd = await _db.Authors
            .Where(el => toAddIds.Contains(el.Id))
            .ToListAsync(ct);

        foreach (var item in authorsToAdd)
            country.Authors.Add(item);        
    }
    private async Task SyncCountryPublishers(Country country, IEnumerable<int> validIds, CancellationToken ct)
    {
        var currentIds = country.Publishers.Select(el => el.Id).ToList();

        var toRemove = country.Publishers
            .Where(el => !validIds.Contains(el.Id))
            .ToList();

        foreach(var item in toRemove)
            country.Publishers.Remove(item);

        var toAddIds = validIds.Except(currentIds);

        var publishersToAdd = await _db.Publishers
            .Where(el => toAddIds.Contains(el.Id))
            .ToListAsync(ct);

        foreach (var item in publishersToAdd)
            country.Publishers.Add(item);        
    }
}