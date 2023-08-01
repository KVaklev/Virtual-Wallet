using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class CurrencyDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.CurrencyNameMinLength, ErrorMessage = Constants.CurrencyMinLengthErrorMessage)]
        [MaxLength(Constants.CurrencyNameMaxLength, ErrorMessage = Constants.CurrencyMaxLengthErrorMessage)]
        public string Name { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.CurrencyCodeLengthErrorMessage)]
        public string CurrencyCode { get; set; }
    }
}
