using AutoMapper;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Models;

namespace hw_2_2_3_26.Profiles;

public class BookProfile : Profile
{
    public BookProfile()
    {
        // COUNTRY
        CreateMap<Country, CountrySummaryDto>();
        CreateMap<Country, CountryDetailDto>()
            .ForMember(dest => dest.Authors, opts =>
            {
                opts.MapFrom(src => src.Authors);
            })
            .ForMember(dest => dest.Publishers, opts =>
            {
                opts.MapFrom(src => src.Publishers);
            });
        CreateMap<CreateCountryRequest, Country>()
            .ForMember(dest => dest.Authors, opts => opts.Ignore())
            .ForMember(dest => dest.Publishers, opts => opts.Ignore());
        CreateMap<UpdateCountryRequest, Country>()
            .ForMember(dest => dest.Authors, opts => opts.Ignore())
            .ForMember(dest => dest.Publishers, opts => opts.Ignore());
        CreateMap<PartialUpdateCountryRequest, Country>()
            .ForMember(dest => dest.Authors, opts => opts.Ignore())
            .ForMember(dest => dest.Publishers, opts => opts.Ignore())
            .ForAllMembers(opts =>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        // AUTHOR
        CreateMap<Author, AuthorSummaryDto>();

        CreateMap<Author, AuthorDetailDto>()
            .ForMember(dest => dest.Country,
                opts => opts.Condition(src => src.Country != null))
            .ForMember(dest => dest.Books,
                opts => opts.MapFrom(src => src.BookAuthors.Select(ba => ba.Book)));

        CreateMap<CreateAuthorRequest, Author>()
            .ForMember(dest => dest.BookAuthors, opts => opts.Ignore());

        CreateMap<UpdateAuthorRequest, Author>()
            .ForMember(dest => dest.BookAuthors, opts => opts.Ignore());

        CreateMap<PartialUpdateAuthorRequest, Author>()
            .ForMember(dest => dest.CountryId,
                opts => opts.Condition(src => src.CountryId.HasValue))
            .ForMember(dest => dest.BookAuthors, opts => opts.Ignore())
            .ForAllMembers(opts =>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        // GENRE
        CreateMap<Genre, GenreSummaryDto>();
        CreateMap<Genre, GenreDetailDto>()
            .ForMember(dest => dest.Books, opts =>
            {
                opts.MapFrom(src => src.BookGenres.Select(el => el.Book));
            });
        CreateMap<CreateGenreRequest, Genre>()
            .ForMember(dest => dest.BookGenres, opts => opts.Ignore());
        CreateMap<UpdateGenreRequest, Genre>()
            .ForMember(dest => dest.BookGenres, opts => opts.Ignore());
        CreateMap<PartialUpdateGenreRequest, Genre>()
            .ForMember(dest => dest.BookGenres, opts => opts.Ignore())
            .ForAllMembers(opts =>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        // PUBLISHER
        CreateMap<Publisher, PublisherSummaryDto>();
        CreateMap<Publisher, PublisherDetailDto>()
            .ForMember(dest => dest.Books, opts =>
                opts.MapFrom(src => src.Books))
            .ForMember(dest => dest.Country, opts =>
                opts.MapFrom(src => src.Country));
        CreateMap<CreatePublisherRequest, Publisher>()
            .ForMember(dest => dest.Country, opts => opts.Ignore())
            .ForMember(dest => dest.Books, opts => opts.Ignore());
        CreateMap<UpdatePublisherRequest, Publisher>()
            .ForMember(dest => dest.Books, opts => opts.Ignore())
            .ForMember(dest => dest.CountryId, opts => opts.Ignore());
        CreateMap<PartialUpdatePublisherRequest, Publisher>()
            .ForMember(dest => dest.Books, opts => opts.Ignore())
            .ForMember(dest => dest.CountryId, opts => opts.Ignore())
            .ForAllMembers(opts =>
            {
                opts.Condition((src, dest, srcMember) => srcMember != null);
            });

        // COVER
        CreateMap<Cover, CoverSummaryDto>();

        // BOOK
        CreateMap<Book, BookDetailDto>()
            .ForMember(dest => dest.Publisher,
                opts => opts.MapFrom(src => src.Publisher))
            .ForMember(dest => dest.Authors,
                opts => opts.MapFrom(src => src.BookAuthors
                    .Select(ba => ba.Author)))
            .ForMember(dest => dest.Genres,
                opts => opts.MapFrom(src => src.BookGenres
                    .Select(bg => bg.Genre)))
            .ForMember(dest => dest.Covers,
                opts => opts.MapFrom(src => src.Covers));

        CreateMap<Book, BookSummaryDto>()
            .ForMember(dest => dest.Covers, opts =>
                opts.MapFrom(src => src.Covers));

        CreateMap<CreateBookRequest, Book>()
            .ForMember(dest => dest.CreatedAt,
                opts => opts.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.BookAuthors, opts => opts.Ignore())
            .ForMember(dest => dest.BookGenres, opts => opts.Ignore())
            .ForMember(dest => dest.Covers, opts => opts.Ignore());

        CreateMap<UpdateBookRequest, Book>()
            .ForMember(dest => dest.Title, opts =>
                {
                    opts.PreCondition(src => src.Title != null);
                    opts.MapFrom(src => src.Title);
                })
            .ForMember(dest => dest.Year, opts =>
                {
                    opts.PreCondition(src => src.Year.HasValue);
                    opts.MapFrom(src => src.Year.Value);
                })
            .ForMember(dest => dest.PageCount, opts =>
                {
                    opts.PreCondition(src => src.PageCount.HasValue);
                    opts.MapFrom(src => src.PageCount.Value);
                })
            .ForMember(dest => dest.PublisherId, opts =>
                {
                    opts.PreCondition(src => src.PublisherId.HasValue);
                    opts.MapFrom(src => src.PublisherId);
                })
            .ForMember(dest => dest.BookAuthors, opts => opts.Ignore())
            .ForMember(dest => dest.BookGenres, opts => opts.Ignore())
            .ForMember(dest => dest.Covers, opts => opts.Ignore());
    }
}