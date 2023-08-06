using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ValidationAttributes
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string phoneNumber = value.ToString();

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return new ValidationResult(Constants.PhoneNumberEmptyFieldErrorMessage);
            }

            if (!IsDigitsOnly(phoneNumber))
            {
                return new ValidationResult(Constants.PhoneNumberDigitContainErrorMessage);
            }

            return ValidationResult.Success;
        }

        private bool IsDigitsOnly(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}