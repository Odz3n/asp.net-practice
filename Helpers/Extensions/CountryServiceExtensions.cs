using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Helpers.Extensions;

public static class CountryServiceExtensions
{
    public static IQueryable<Country> ApplyFilters(
        this IQueryable<Country> query,
        CountryGetParameters parameters
    )
    {
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            query = query.Where(c =>
                EF.Functions.Like(c.Name, $"%{parameters.Search}%"));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Author))
        {
            query = query.Where(c =>
                c.Authors.Any(a =>
                    EF.Functions.Like(a.FirstName, $"%{parameters.Author}%") ||
                    EF.Functions.Like(a.LastName, $"%{parameters.Author}%")
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(parameters.Publisher))
        {
            query = query.Where(c =>
                c.Publishers.Any(p =>
                    EF.Functions.Like(
                        p.Name,
                        $"%{parameters.Publisher}%"
                    )
                )
            );
        }

        return query;
    }
    public static IQueryable<Country> ApplySorting(
        this IQueryable<Country> query,
        CountryGetParameters parameters
    )
    {
        return parameters.Sort switch
        {
            "name_asc" => query.OrderBy(c => c.Name),
            "name_desc" => query.OrderByDescending(c => c.Name),
            _ => query.OrderBy(c => c.Id)
        };
    }
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        CountryGetParameters parameters,
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