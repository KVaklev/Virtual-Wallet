using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class UpdateCardDto
    {
        [CardNumber(ErrorMessage = Constants.CardNumberFieldErroMessage)]
        [StringLength(16, ErrorMessage = Constants.CardNumberLengthErrorMessage)]
        public string? CardNumber { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [MinLength(Constants.CardHolderMinLength, ErrorMessage = Constants.CardHolderMinLengthErrorMessage)]
        [MaxLength(Constants.CardHolderMaxLength, ErrorMessage = Constants.CardHolderMaxLengthErrorMessage)]
        public string? CardHolder { get; set; }

        [CardCheckNumber(ErrorMessage = Constants.CardCheckNumberFieldErrorMessage)]
        [StringLength(Constants.CheckNumberLength, ErrorMessage = Constants.CheckNumberLengthErrorMessage)]
        public string? CheckNumber { get; set; }

        [AllowedCardTypes(ErrorMessage = Constants.EmptyFieldCardTypeErrorMessage)]
        public string? CardType { get; set; }
        public decimal? CreditLimit { get; set; }

        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.CurrencyCodeLengthErrorMessage)]
        public string? CurrencyCode { get; set; }
    }
}
