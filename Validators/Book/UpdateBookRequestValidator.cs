using FluentValidation;
using hw_2_2_3_26.DTO;
using hw_2_2_3_26.Services;

namespace hw_2_2_3_26.Validators;

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    private readonly IPublisherService _publisherService;
    private readonly IAuthorService _authorService;
    private readonly IGenreService _genreService;

    public UpdateBookRequestValidator(
        IPublisherService publisherService,
        IAuthorService authorService,
        IGenreService genreService)
    {
        _publisherService = publisherService;
        _authorService = authorService;
        _genreService = genreService;

        When(b => b.Title != null, () =>
        {
            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title length cannot be longer than 100 characters.");

        });

        When(b => b.Year != null, () =>
        {
            RuleFor(b => b.Year)
                .NotNull().WithMessage("Year can not be null.")
                .NotEmpty().WithMessage("Year can not be empty.")
                .InclusiveBetween(1000, DateTime.UtcNow.Year)
                    .WithMessage($"Year must be between 1000 and {DateTime.UtcNow.Year}.")
                .Must(year => year <= DateTime.UtcNow.Year)
                    .WithMessage("Year cannot be in the future.");
        });

        When(b => b.PageCount != null, () =>
        {
            RuleFor(b => b.PageCount)
                .GreaterThan(0).WithMessage("Page count must be greater than zero.");
        });

        When(b => b.PublisherId != null, () =>
        {
            RuleFor(b => b.PublisherId)
                .GreaterThan(0).WithMessage("Publisher ID must be greater than zero.")
                .MustAsync(async (publisherId, ct) => await _publisherService.PublisherExists(publisherId, ct))
                    .WithMessage("Publisher with ID {PropertyValue} does not exist.");

        });

        When(b => b.AuthorIds != null && b.AuthorIds.Any(), () =>
        {
            RuleFor(b => b.AuthorIds)
                .Must(ids => ids!.Distinct().Count() == ids.Count)
                    .WithMessage("Duplicate author IDs are not allowed.");

            RuleForEach(b => b.AuthorIds)
                .GreaterThan(0).WithMessage("Author ID must be greater than zero.")
                .MustAsync(async (authorId, ct) => await _authorService.AuthorExists(authorId, ct))
                    .WithMessage("Author with ID {PropertyValue} does not exist.");
        });

        When(b => b.GenreIds != null && b.GenreIds.Any(), () =>
        {
            RuleFor(b => b.GenreIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                    .WithMessage("Duplicate genre IDs are not allowed.");

            RuleForEach(b => b.GenreIds)
                .GreaterThan(0).WithMessage("Genre ID must be greater than zero.")
                .MustAsync(async (genreId, ct) => await _genreService.GenreExists(genreId, ct))
                    .WithMessage("Genre with ID {PropertyValue} does not exist.");
        });

        When(b => b.Covers != null && b.Covers.Any(), () =>
        {
            RuleForEach(b => b.Covers)
                .Must(c => c.Length > 0).WithMessage("Cover file can not be empty.")
                .Must(c => c.Length <= 5 * 1024 * 1024).WithMessage("Each cover file must be less than 5 MB.")
                .Must(c => new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
                    .Contains(Path.GetExtension(c.FileName).ToLowerInvariant()))
                    .WithMessage("Only .jpg, .jpeg, .png, .gif, or .webp files are allowed.")
                .Must(c => new[] { "image/jpeg", "image/png", "image/gif", "image/webp" }
                    .Contains(c.ContentType))
                    .WithMessage("Only JPEG, PNG, GIF or WebP images are allowed.");
        });
    }
}