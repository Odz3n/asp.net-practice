namespace hw_2_2_3_26.Helpers.Pagination;

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount/(double)PageSize);
    public bool HasPrevPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
};