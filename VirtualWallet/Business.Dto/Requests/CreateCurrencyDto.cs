using DataAccess.Models.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class CreateCurrencyDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.CurrencyNameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.CurrencyNameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Name { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.CurrencyNameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.CurrencyNameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Country { get; set; }

        
    }
}
