using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ValidationAttributes
{
    public class PasswordAttribute : ValidationAttribute
    {
        private const int MinLength = 8;
        private const string SpecialCharacters = "!@#$%^&*()-_=+[]{}|\\;:'\",.<>~?/";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (password == null)
            {
                return ValidationResult.Success;
            }

            if (password.Length < MinLength)
            {
                return new ValidationResult(Constants.PasswordMinLengthErrorMessage);
            }

            if (!password.Any(char.IsLower))
            {
                return new ValidationResult(Constants.PasswordLowerCaseErrorMessage);
            }

            if (!password.Any(char.IsUpper))
            {
                return new ValidationResult(Constants.PasswordUpperCaseErrorMessage);
            }

            if (!password.Any(char.IsDigit))
            {
                return new ValidationResult(Constants.PasswordDigitContainErrorMessage);
            }

            if (!password.Any(c => SpecialCharacters.Contains(c)))
            {
                return new ValidationResult(Constants.PasswordSpecialCharacterContainErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
