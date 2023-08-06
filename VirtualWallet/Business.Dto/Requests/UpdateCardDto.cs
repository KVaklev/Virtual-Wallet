using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class UpdateCardDto
    {
        [CardNumber(ErrorMessage = Constants.CardNumberFieldErroMessage)]
        [StringLength(16, ErrorMessage = Constants.LengthErrorMessage)]
        public string? CardNumber { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [MinLength(Constants.CardHolderMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.CardHolderMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? CardHolder { get; set; }

        [CardNumber(ErrorMessage = Constants.CardCheckNumberFieldErrorMessage)]
        [StringLength(Constants.CheckNumberLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string? CheckNumber { get; set; }

        [AllowedCardTypes(ErrorMessage = Constants.EmptyFieldCardTypeErrorMessage)]
        public string? CardType { get; set; }
        public decimal? CreditLimit { get; set; }

        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string? CurrencyCode { get; set; }
    }
}
