using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class CreateTransactionDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.UsernameMinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.UsernameMaxLengthErrorMessage)]
        public string RecepientUsername { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.CurrencyCodeLengthErrorMessage)]
        public string CurrencyCode { get; set; }
    }
}
