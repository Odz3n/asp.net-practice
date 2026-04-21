using AutoMapper;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Helpers.Extensions;
using hw_2_2_3_26.Helpers.Pagination;
using hw_2_2_3_26.Models;
using hw_2_2_3_26.Repository;
using MediatR;

namespace hw_2_2_3_26.Features.Books.Queries;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, PagedResult<BookSummaryDto>>
{
    private readonly IMapper _mapper;
    private readonly IBookRepository _bookRepository;
    public GetAllBooksQueryHandler(
        IMapper mapper,
        IBookRepository bookRepository)
    {
        _mapper = mapper;
        _bookRepository = bookRepository;
    }

    public async Task<PagedResult<BookSummaryDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var query = _bookRepository.GetBooksAsync(request.Parameters, cancellationToken);
        return await query
            .ToPagedResultAsync<Book, BookSummaryDto>(
                request.Parameters,
                _mapper.ConfigurationProvider,
                cancellationToken
            );
    }
}