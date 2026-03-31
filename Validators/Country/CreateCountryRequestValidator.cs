using FluentValidation;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Services;

namespace hw_2_2_3_26.Validators;

public class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
{
    private readonly IAuthorService _authorService;
    private readonly IPublisherService _publisherService;
    public CreateCountryRequestValidator(IAuthorService authorService, IPublisherService publisherService)
    {
        _authorService = authorService;
        _publisherService = publisherService;

        RuleFor(c => c.Name)
            .NotNull().WithMessage("Name cannot be null")
            .NotEmpty().WithMessage("Name cannot be empty")
            .MaximumLength(60).WithMessage("Name length cannot be longer than 60 characters");

        When(c => c.AuthorIds != null && c.AuthorIds.Any(), () =>
        {
            RuleFor(c => c.AuthorIds)
                .Must(ids => ids.Distinct().Count() == ids.Count())
                .WithMessage("Duplicate author IDs are not allowed");

            RuleForEach(c => c.AuthorIds)
                .GreaterThan(0).WithMessage("Author ID must be greater than zero")
                .MustAsync(async (id, ct) => await _authorService.AuthorExists(id, ct))
                .WithMessage("Author with ID {PropertyValue} does not exist");
        });

        When(c => c.PublisherIds != null && c.PublisherIds.Any(), () =>
        {
            RuleFor(c => c.PublisherIds)
                .Must(ids => ids.Distinct().Count() == ids.Count())
                .WithMessage("Duplicate publisher IDs are not allowed");

            RuleForEach(c => c.PublisherIds)
                .GreaterThan(0).WithMessage("Publisher ID must be greater than zero")
                .MustAsync(async (id, ct) => await _publisherService.PublisherExists(id, ct))
                .WithMessage("Publisher with ID {PropertyValue} does not exist");
        });
    }
}