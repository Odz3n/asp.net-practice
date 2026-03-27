using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Helpers.Extensions;

public static class AuthorServiceExtensions
{
    public static IQueryable<Author> ApplyFilters(
        this IQueryable<Author> query,
        AuthorGetParameters parameters
    )
    {
        if (parameters.Search != null)
            query = query
                .Where(el => EF.Functions.Like(el.FirstName + " " + el.LastName, $"%{parameters.Search}%"));
        if (parameters.Country != null)
            query = query
                .Where(el => el.Country != null &&
                    EF.Functions.Like(el.Country.Name, parameters.Country));
        if (parameters.Book != null)
            query = query
                .Where(el => el.BookAuthors.Any(ba => 
                    EF.Functions.Like(ba.Book.Title, $"%{parameters.Book}%")));
        if (parameters.DateOfBirth != null)
            query = query
                .Where(el => el.DateOfBirth.Equals(parameters.DateOfBirth));
        return query;
    }
    public static IQueryable<Author> ApplySorting(
        this IQueryable<Author> query,
        AuthorGetParameters parameters
    )
    {
        return parameters.Sort switch
        {
            "fname_asc" => query.OrderBy(el => el.FirstName),
            "fname_desc" => query.OrderByDescending(el => el.FirstName),
            "lname_asc" => query.OrderBy(el => el.LastName),
            "lname_desc" => query.OrderByDescending(el => el.LastName),
            "bdate_asc" => query.OrderBy(el => el.DateOfBirth),
            "bdate_desc" => query.OrderByDescending(el => el.DateOfBirth),
            _ => query.OrderBy(el => el.Id)
        };
    }
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        AuthorGetParameters parameters,
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