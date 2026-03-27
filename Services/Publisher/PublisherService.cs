using AutoMapper;
using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Services;

public class PublisherService : IPublisherService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;
    public PublisherService(IMapper mapper, AppDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }
    public async Task<PublisherSummaryDto> Create(CreatePublisherRequest request, CancellationToken ct)
    {
        var newPublisher = _mapper.Map<Publisher>(request);

        var validBookIds = await GetValidBookIds(request.BookIds, ct);
        await SyncPublisherBooks(newPublisher, validBookIds, ct);

        var validCountryId = await GetValidCountryId(request.CountryId, ct);
        await SyncPublisherCountry(newPublisher, validCountryId, ct);

        await _db.AddAsync(newPublisher, ct);

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<PublisherSummaryDto>(newPublisher);
    }

    public async Task<bool> Delete(int id, CancellationToken ct)
    {
        var target = await _db.Publishers.FirstOrDefaultAsync(el => el.Id == id, ct);
        if (target == null)
            return false;

        _db.Publishers.Remove(target);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PagedResult<PublisherSummaryDto>> GetAllPublishers(PublisherGetParameters parameters, CancellationToken ct)
    {
        return await _db.Publishers
            .AsNoTracking()
            .ApplyFilters(parameters)
            .ApplySorting(parameters)
            .ToPagedResultAsync<Publisher, PublisherSummaryDto>(parameters, _mapper.ConfigurationProvider, ct);
    }

    public async Task<PublisherDetailDto?> GetPublisherById(int id, CancellationToken ct)
    {
        return await _db.Publishers
            .AsNoTracking()
            .Where(el => el.Id == id)
            .ProjectTo<PublisherDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<PublisherDetailDto>> GetPublisherBySearchParameters(PublisherSearchParameters parameters, CancellationToken ct)
    {
        var query = _db.Publishers
            .AsNoTracking();

        if (parameters.Name != null)
            query = query
                .Where(el => EF.Functions.Like(el.Name, $"%{parameters.Name}%"));

        if (parameters.FoundationYear.HasValue)
            query = query
                .Where(el => el.FoundedAt != null && el.FoundedAt.Value.Year == parameters.FoundationYear);

        if (parameters.Country != null)
            query = query
                .Where(el => el.Country != null && EF.Functions.Like(el.Country.Name, $"%{parameters.Country}%"));

        return await query
            .ProjectTo<PublisherDetailDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> PartialUpdate(int id, PartialUpdatePublisherRequest request, CancellationToken ct)
    {
        var target = await _db.Publishers
            .Where(el => el.Id == id)
            .Include(el => el.Books)
            .Include(el => el.Country)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        if (request.BookIds != null)
        {
            var validBookIds = await GetValidBookIds(request.BookIds, ct);
            await SyncPublisherBooks(target, validBookIds, ct);
        }

        if (request.CountryId.HasValue)
        {
            var validCountryId = await GetValidCountryId(request.CountryId.Value, ct);
            await SyncPublisherCountry(target, validCountryId, ct);
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Update(int id, UpdatePublisherRequest request, CancellationToken ct)
    {
        var target = await _db.Publishers
            .Where(el => el.Id == id)
            .Include(el => el.Books)
            .Include(el => el.Country)
            .FirstOrDefaultAsync(ct);

        if (target == null)
            return false;

        _mapper.Map(request, target);

        var validBookIds = await GetValidBookIds(request.BookIds, ct);
        await SyncPublisherBooks(target, validBookIds, ct);

        var validCountryId = await GetValidCountryId(request.CountryId, ct);
        await SyncPublisherCountry(target, validCountryId, ct);

        await _db.SaveChangesAsync(ct);
        return true;
    }
    private async Task<IEnumerable<int>> GetValidBookIds(IEnumerable<int> ids, CancellationToken ct)
    {
        return await _db.Books
            .Where(el => ids.Contains(el.Id))
            .Select(el => el.Id)
            .ToListAsync(ct);
    }
    private async Task<int?> GetValidCountryId(int id, CancellationToken ct)
    {
        var exists = await _db.Countries
        .AnyAsync(el => el.Id == id, ct);

        return exists ? id : null;
    }

    private async Task SyncPublisherBooks(Publisher publisher, IEnumerable<int> validIds, CancellationToken ct)
    {
        var currentIds = publisher.Books.Select(el => el.Id).ToList();

        var booksToRemove = publisher.Books
            .Where(el => !validIds.Contains(el.Id))
            .ToList();
        foreach (var item in booksToRemove)
            publisher.Books.Remove(item);

        var toAddIds = validIds.Except(currentIds);
        var booksToAdd = await _db.Books
            .Where(el => toAddIds.Contains(el.Id))
            .ToListAsync(ct);

        foreach (var item in booksToAdd)
            publisher.Books.Add(item);
    }
    private Task SyncPublisherCountry(Publisher publisher, int? validId, CancellationToken ct)
    {
        if (validId == null)
        {
            publisher.CountryId = null;
            publisher.Country = null;
            return Task.CompletedTask;
        }

        if (publisher.CountryId == validId)
            return Task.CompletedTask;

        publisher.CountryId = validId;

        publisher.Country = null;

        return Task.CompletedTask;
    }
}