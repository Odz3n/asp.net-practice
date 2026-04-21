using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Helpers.QueryParameters;
using MediatR;

namespace hw_2_2_3_26.Features.Books.Queries;

public record GetAllBooksQuery(BookGetParameters Parameters)
    : IRequest<PagedResult<BookSummaryDto>>;