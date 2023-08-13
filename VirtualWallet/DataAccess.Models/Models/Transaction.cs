using DataAccess.Models.Enums;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class Transaction

    {   
        public int Id { get; set; }
  
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int AccountSenderId { get; set; } 

        [JsonIgnore]
        public Account AccountSender { get; set; }
        
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int AccountRecepientId { get; set; } 

        [JsonIgnore]
        public Account AccountRecipient { get; set; }

        public DirectionType Direction { get; set; }
      
        [Range(Constants.MinAmount, Constants.MaxAmount, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal Amount { get; set; }

        [Range(Constants.MinAmount, Constants.MaxAmount, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal? AmountExchange { get; set; }

        [Range(Constants.MinAmount, Constants.MaxAmount, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public decimal ExchangeRate { get; set; } 

        public DateTime Date { get; set; }

        
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int CurrencyId { get; set; } 
        
        [JsonIgnore]
        public Currency Currency { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.DescriptionMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.DescriptionMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Description { get; set; }

    }
}
