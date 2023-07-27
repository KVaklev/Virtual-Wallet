using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Business.DTOs
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
        public string? CardType { get; set; }
        public decimal? CreditLimit { get; set; }
        public string? Currency { get; set; }

    }
}
