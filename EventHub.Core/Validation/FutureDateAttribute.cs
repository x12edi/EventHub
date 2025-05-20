using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.Validation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date < DateTime.UtcNow.Date)
                {
                    return new ValidationResult("Start date must be today or in the future.");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid date format.");
        }
    }
}