using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class CreateCardDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [CardNumber(ErrorMessage = Constants.CardNumberFieldErroMessage)]
        [StringLength(16, ErrorMessage = Constants.CardNumberLengthErrorMessage)]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldExpirationDateErrorMessage)]
        [ValidExpirationDate(ErrorMessage = Constants.ExpirationDateErrorMessage)]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.CardHolderMinLength, ErrorMessage = Constants.CardHolderMinLengthErrorMessage)]
        [MaxLength(Constants.CardHolderMaxLength, ErrorMessage = Constants.CardHolderMaxLengthErrorMessage)]
        public string CardHolder { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [CardCheckNumber(ErrorMessage = Constants.CardCheckNumberFieldErrorMessage)]
        [StringLength(Constants.CheckNumberLength, ErrorMessage = Constants.CheckNumberLengthErrorMessage)]
        public string CheckNumber { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldCardTypeErrorMessage)]
        public string CardType { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.UsernameMinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.UsernameMaxLengthErrorMessage)]
        public string AccountUsername { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public decimal? Balance { get; set; }
        public decimal? CreditLimit { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public string CurrencyCode { get; set; }
    }
}
