using DataAccess.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class Transfer
    {

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int AccountId { get; set; }

        [JsonIgnore]
        public Account Account { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int CurrencyId { get; set; }

        [JsonIgnore]
        public Currency Currency { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(0, double.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public decimal Amount { get; set; }
        public TransferDirection TransferType { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int CardId { get; set; }

        [JsonIgnore]
        public Card Card { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsCancelled { get; set; }


    }
}
