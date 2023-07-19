using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class PasswordAttribute : ValidationAttribute
    {
        private const int MinLength = 8;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (password == null)
            {
                return ValidationResult.Success;
            }

            if (password.Length < MinLength)
            {
                return new ValidationResult($"Password must be at least {MinLength} characters long.");
            }

            if (!password.Any(char.IsLower))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (!password.Any(char.IsUpper))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(char.IsDigit))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }

            return ValidationResult.Success;
        }
    }
}
