using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Helpers.Extensions;

public static class PublisherServiceExtensions
{
    public static IQueryable<Publisher> ApplyFilters(
        this IQueryable<Publisher> query,
        PublisherGetParameters parameters
    )
    {
        if (parameters.Search != null)
            query = query
                .Where(el => EF.Functions.Like(el.Name, $"%{parameters.Search}%"));
        if (parameters.Country != null)
            query = query
                .Where(el => el.Country != null &&
                    EF.Functions.Like(el.Country.Name, parameters.Country));
        if (parameters.Book != null)
            query = query
                .Where(el => el.Books.Any(b => EF.Functions.Like(b.Title, $"%{parameters.Book}%")));
        if (parameters.FoundationDate != null)
            query = query
                .Where(el => el.FoundedAt.Equals(parameters.FoundationDate));
        return query;
    }
    public static IQueryable<Publisher> ApplySorting(
        this IQueryable<Publisher> query,
        PublisherGetParameters parameters
    )
    {
        return parameters.Sort switch
        {
            "name_asc" => query.OrderBy(el => el.Name),
            "name_desc" => query.OrderByDescending(el => el.Name),
            "est_asc" => query.OrderBy(el => el.FoundedAt),
            "est_desc" => query.OrderByDescending(el => el.FoundedAt),
            _ => query.OrderBy(el => el.Id)
        };
    }
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        PublisherGetParameters parameters,
        AutoMapper.IConfigurationProvider mapperConfig,
        CancellationToken token
    )
    {
        var page = Math.Max(1, parameters.Page);
        var size = Math.Clamp(parameters.Size, 1, 100);

        int totalCount = await query.CountAsync(token);

        var items = await query.Skip((page - 1) * size)
            .Take(size)
            .ProjectTo<TDto>(mapperConfig)
            .ToListAsync(token);

        return new PagedResult<TDto>(items, totalCount, page, size);
    }
}