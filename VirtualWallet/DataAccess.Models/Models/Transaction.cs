using DataAccess.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class Transaction

    {
        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int AccountSenderId { get; set; } //FK

        [JsonIgnore]
        public Account AccountSender { get; set; }

        public int AccountRecepientId { get; set; } //FK

        [JsonIgnore]
        public Account AccountRecepient { get; set; }

        public DirectionType Direction { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(0, double.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }


        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int CurrencyId { get; set; } //FK
        
        [JsonIgnore]
        public Currency Currency { get; set; }

        public bool IsExecuted { get; set; }

        public bool IsDeleted { get; set; }

    }
}
