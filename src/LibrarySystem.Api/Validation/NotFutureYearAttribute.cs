using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.Validation;

public class NotFutureYearAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not int year)
        {
            return false;
        }

        return year <= DateTime.UtcNow.Year;
    }
}