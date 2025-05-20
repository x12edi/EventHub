using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EventHub.Core.Validation
{
    public class TitleFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string title)
            {
                if (string.IsNullOrWhiteSpace(title) || title.Length < 3 || title.Length > 100)
                {
                    return new ValidationResult("Title must be between 3 and 100 characters.");
                }
                if (!Regex.IsMatch(title, @"^[a-zA-Z0-9\s]+$"))
                {
                    return new ValidationResult("Title can only contain letters, numbers, and spaces.");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid title format.");
        }
    }
}