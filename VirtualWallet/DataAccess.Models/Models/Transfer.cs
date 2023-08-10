using DataAccess.Models.Enums;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int AccountId { get; set; }

        [JsonIgnore]
        public Account Account { get; set; }

        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int CurrencyId { get; set; }

        [JsonIgnore]
        public Currency Currency { get; set; }

        [Range(Constants.MinAmount, Constants.MaxAmount, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal Amount { get; set; }
        public TransferDirection TransferType { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public int CardId { get; set; }

        [JsonIgnore]
        public Card Card { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsCancelled { get; set; }


    }
}
