using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class Currency
    {
        public int Id { get; set; }

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


       // public byte[] FlagImage { get; set; }


        public List<Account> Accounts { get; set; } = new List<Account>();
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public bool IsDeleted { get; set; }


    }
}
