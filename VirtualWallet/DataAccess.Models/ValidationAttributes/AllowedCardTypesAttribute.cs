using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ValidationAttributes
{
    public class AllowedCardTypesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string? cardType = value as string;

            if (value == null)
            {
                return ValidationResult.Success;
            }

            string[] allowedCardTypes = new string[] { "Credit", "Debit" };

            if (Array.IndexOf(allowedCardTypes, cardType) == -1)
            {
                return new ValidationResult(Constants.EmptyFieldCardTypeErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
}
