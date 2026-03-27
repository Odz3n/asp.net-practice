using AutoMapper.QueryableExtensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using Microsoft.EntityFrameworkCore;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Helpers.Extensions;

public static class BookServiceExtensions
{
    public static IQueryable<Book> ApplyFilters(
        this IQueryable<Book> query,
        BookGetParameters parameters
    )
    {
        if (parameters.Search != null)
            query = query
                .Where(el => EF.Functions.Like(el.Title, $"%{parameters.Search}%"));
        if (parameters.Author != null)
            query = query
                .Where(el => el.BookAuthors.Any(ba =>
                    EF.Functions.Like(ba.Author.FirstName + " " + ba.Author.LastName,
                    $"%{parameters.Author}%")));
        if (parameters.Publisher != null)
            query = query
                .Where(el => el.Publisher != null &&
                    EF.Functions.Like(el.Publisher.Name, parameters.Publisher));
        if (parameters.Genre != null)
            query = query
                .Where(el => el.BookGenres.Any(bg =>
                    EF.Functions.Like(bg.Genre.Name, $"%{parameters.Genre}%")));
        if (parameters.PageCount.HasValue)
            query = query
                .Where(el => el.PageCount == parameters.PageCount);

        return query;
    }
    public static IQueryable<Book> ApplySorting(
        this IQueryable<Book> query,
        BookGetParameters parameters
    )
    {
        return parameters.Sort switch
        {
            "title_asc" => query.OrderBy(el => el.Title),
            "title_desc" => query.OrderByDescending(el => el.Title),
            "pages_asc" => query.OrderBy(el => el.PageCount),
            "pages_desc" => query.OrderByDescending(el => el.PageCount),
            _ => query.OrderBy(el => el.Id)
        };
    }
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        BookGetParameters parameters,
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