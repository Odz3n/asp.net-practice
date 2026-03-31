using FluentValidation;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Services;

namespace hw_2_2_3_26.Validators;

public class PartialUpdateGenreRequestValidator : AbstractValidator<PartialUpdateGenreRequest>
{
    private readonly IBookService _bookService;
    public PartialUpdateGenreRequestValidator(IBookService bookService)
    {
        _bookService = bookService;

        When(g => g.Name != null, () =>
        {
            RuleFor(g => g.Name)
                .NotNull().WithMessage("Name can not be null")
                .NotEmpty().WithMessage("Name can not be empty")
                .MaximumLength(50).WithMessage("Name length cannot be longer than 50 characters.");
        });

        When(g => g.BookIds != null && g.BookIds.Any(), () =>
        {
            RuleForEach(g => g.BookIds)
                .GreaterThan(0)
                    .WithMessage("Book ID must be greater than zero")
                .MustAsync(async (bookId, ct) =>
                {
                    return await _bookService.BookExists(bookId, ct);
                }).WithMessage("Book with ID {PropertyValue} does not exist");
        });
    }
}