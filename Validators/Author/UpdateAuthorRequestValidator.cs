using FluentValidation;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Services;

namespace hw_2_2_3_26.Validators;

public class UpdateAuthorRequestValidator : AbstractValidator<UpdateAuthorRequest>
{
    private readonly ICountryService _countryService;
    private readonly IBookService _bookService;
    public UpdateAuthorRequestValidator(ICountryService countryService, IBookService bookService)
    {
        _countryService = countryService;
        _bookService = bookService;

        RuleFor(a => a.FirstName)
            .NotNull().WithMessage("Firstname can not be null")
            .NotEmpty().WithMessage("Firstname can not be empty")
            .MaximumLength(50).WithMessage("Firstname length can not be greater than 50")
            .MinimumLength(1).WithMessage("Firstname length can not be less than 1");
        RuleFor(a => a.LastName)
            .NotNull().WithMessage("Lastname can not be null")
            .NotEmpty().WithMessage("Lastname can not be empty")
            .MaximumLength(50).WithMessage("Lastname length can not be greater than 50")
            .MinimumLength(1).WithMessage("Lastname length can not be less than 1");
        RuleFor(a => a.DateOfBirth)
            .NotNull().WithMessage("Date of birth can not be null")
            .NotEmpty().WithMessage("Date of birth can not be empty")
            .InclusiveBetween(DateTime.Parse("1900-01-01"), DateTime.Parse($"{DateTime.UtcNow.Year - 18}-01-01"))
                .WithMessage($"Date of birth must be between {DateTime.Parse("1900-01-01").ToShortDateString()} and {DateTime.Parse($"{DateTime.UtcNow.Year - 18}-01-01").ToShortDateString()}");
        When(a => a.CountryId != null, () =>
        {
            RuleFor(a => a.CountryId)
                .GreaterThan(0)
                    .WithMessage("Country ID must be greater than zero")
                .MustAsync(async (countryId, ct) =>
                {
                    return await _countryService.CountryExists(countryId, ct);
                }).WithMessage("Country with ID {PropertyValue} does not exist");
        });
        When(a => a.BookIds != null && a.BookIds.Any(), () =>
        {
            RuleFor(a => a.BookIds)
                .Must(ids => ids.Distinct().Count() == ids.Count())
                    .WithMessage("Duplicate book IDs are not allowed");

            RuleForEach(a => a.BookIds)
                .GreaterThan(0)
                    .WithMessage("Book ID must be greater than zero")
                .MustAsync(async (bookId, ct) =>
                {
                    return await _bookService.BookExists(bookId, ct);
                }).WithMessage("Book with ID {PropertyValue} does not exist");
        });
    }
}