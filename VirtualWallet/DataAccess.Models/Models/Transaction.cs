using DataAccess.Models.Enums;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class Transaction

    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int Id { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int AccountSenderId { get; set; } //FK

        [JsonIgnore]
        public Account AccountSender { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int AccountRecepientId { get; set; } //FK

        [JsonIgnore]
        public Account AccountRecipient { get; set; }

        public DirectionType Direction { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(Constants.MinAmount, Constants.MaxAmount, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int CurrencyId { get; set; } //FK
        
        [JsonIgnore]
        public Currency Currency { get; set; }

        public bool IsExecuted { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.DescriptionMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.DescriptionMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Description { get; set; }

    }
}
