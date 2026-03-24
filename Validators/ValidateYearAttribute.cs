using System.ComponentModel.DataAnnotations;

namespace MyApp.Validators;

public class ValidateYearAttribute : ValidationAttribute
{
    private readonly int _rangeStart;
    private readonly int _rangeEnd;
    public ValidateYearAttribute(int rangeStart, int rangeEnd)
    {
        _rangeStart = rangeStart;
        _rangeEnd = rangeEnd;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;
            
        if (value is int year)
        {
            if (year < _rangeStart || year > _rangeEnd)
                return new ValidationResult($"Year must be between {_rangeStart} and {_rangeEnd}");
            return ValidationResult.Success;
        }
        return new ValidationResult("Invalid year value");
    }
}