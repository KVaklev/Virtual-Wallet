using DataAccess.Models.Enums;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class Card 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [CardNumber(ErrorMessage = "The card number must contain only digits.")]
        [StringLength(16, ErrorMessage = "The {0} must be exactly {1} characters long.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(30, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string CardHolder { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [CardCheckNumber(ErrorMessage = "The card check number must contain only digits.")]
        [StringLength(3, ErrorMessage = "The {0} must be exactly {1} characters long.")]
        public string CheckNumber { get; set; }
        public CardType CardType { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public decimal? Balance { get; set; }
        public decimal? CreditLimit { get; set; }

        public Currency? Currency { get; set; }

        public int? CurrencyId { get; set; }
    }
}
