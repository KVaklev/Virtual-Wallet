using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ValidationAttributes
{
    public class DateOfBirthAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDayDate)
            {
                if (birthDayDate >= DateTime.Now)
                {
                    return new ValidationResult(Constants.BirthDateErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
