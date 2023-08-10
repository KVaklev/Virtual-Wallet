using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class CreateTransferDto
    {
        [Range(Constants.MinAmount, Constants.MaxAmount, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [CardNumber(ErrorMessage = Constants.CardNumberFieldErroMessage)]
        [StringLength(16, ErrorMessage = Constants.LengthErrorMessage)]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.DescriptionMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.DescriptionMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string TransferType { get; set; }
    }
}
