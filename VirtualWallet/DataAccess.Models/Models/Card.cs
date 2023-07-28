using DataAccess.Models.Enums;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class Card 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [CardNumber(ErrorMessage = Constants.CardNumberFieldErroMessage)]
        [StringLength(16, ErrorMessage = Constants.CardNumberLengthErrorMessage)]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.CardHolderMinLength, ErrorMessage = Constants.CardHolderMinLengthErrorMessage)]
        [MaxLength(Constants.CardHolderMaxLength, ErrorMessage = Constants.CardHolderMaxLengthErrorMessage)]
        public string CardHolder { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [CardCheckNumber(ErrorMessage = Constants.CardCheckNumberFieldErrorMessage)]
        [StringLength(Constants.CheckNumberLength, ErrorMessage = Constants.CheckNumberLengthErrorMessage)]
        public string CheckNumber { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public CardType CardType { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public int AccountId { get; set; }

        [JsonIgnore]
        public Account? Account { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public decimal? Balance { get; set; }
        public decimal? CreditLimit { get; set; }

        [JsonIgnore]
        public Currency? Currency { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public int? CurrencyId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
