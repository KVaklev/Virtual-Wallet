using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ValidationAttributes
{
    public class ValidExpirationDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime expirationDate)
            {
                if (expirationDate <= DateTime.Now)
                {
                    return new ValidationResult(Constants.ExpirationDateErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
