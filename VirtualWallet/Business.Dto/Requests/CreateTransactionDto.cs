using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class CreateTransactionDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string RecepientUsername { get; set; }

        
        [Range(Constants.MinAmount, Constants.MaxAmount, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.DescriptionMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.DescriptionMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Description { get; set; }
    }
}
