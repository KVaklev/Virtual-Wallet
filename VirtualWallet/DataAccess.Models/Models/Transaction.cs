using DataAccess.Models.Enums;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class Transaction

    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(1, int.MaxValue, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int Id { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(1, int.MaxValue, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int AccountSenderId { get; set; } //FK

        [JsonIgnore]
        public Account AccountSender { get; set; }

        public int AccountRecepientId { get; set; } //FK

        [JsonIgnore]
        public Account AccountRecepient { get; set; }

        public DirectionType Direction { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(0, double.MaxValue, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(1, int.MaxValue, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int CurrencyId { get; set; } //FK
        
        [JsonIgnore]
        public Currency Currency { get; set; }

        public bool IsExecuted { get; set; }

        public bool IsDeleted { get; set; }

    }
}
