using System.Data;
using FluentValidation;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Services;

namespace hw_2_2_3_26.Validators;

public class CreatePublisherRequestValidator : AbstractValidator<CreatePublisherRequest>
{
    private readonly IBookService _bookService;
    private readonly ICountryService _countryService;
    public CreatePublisherRequestValidator(IBookService bookService, ICountryService countryService)
    {
        _bookService = bookService;
        _countryService = countryService;

        RuleFor(p => p.Name)
           .NotNull().WithMessage("Name can not be null")
           .NotEmpty().WithMessage("Name can not be empty")
           .MaximumLength(100).WithMessage("Name length cannot be longer than 100 characters.");

        When(p => p.CountryId != null, () =>
        {
            RuleFor(p => p.CountryId)
                .GreaterThan(0)
                    .WithMessage("Publisher ID must be greater than zero")
                .MustAsync(async (countryId, ct) =>
                {
                    return await _countryService.CountryExists(countryId, ct);
                }).WithMessage("Publisher with ID {PropertyValue} does not exist");
        });

        RuleFor(a => a.FoundedAt)
            .NotNull().WithMessage("Foundation date can not be null.")
            .NotEmpty().WithMessage("Foundation date can not be empty.")
            .GreaterThan(DateTime.Parse("1900-01-01"))
                .WithMessage($"Foundation date must be greater {DateTime.Parse("1900-01-01").ToShortDateString()}")
            .Must(date => date <= DateTime.UtcNow)
                .WithMessage("Foundation date can not be in the future");

        When(p => p.BookIds != null && p.BookIds.Any(), () =>
        {
            RuleFor(p => p.BookIds)
                .Must(ids => ids.Distinct().Count() == ids.Count())
                .WithMessage("Duplicate book IDs are not allowed");

            RuleForEach(p => p.BookIds)
                .GreaterThan(0)
                    .WithMessage("Book ID must be greater than zero")
                .MustAsync(async (bookId, ct) =>
                {
                    return await _bookService.BookExists(bookId, ct);
                }).WithMessage("Book with ID {PropertyValue} does not exist");
        });
    }
}