using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class History 
    {
        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int Id { get; set; } 

        public DateTime EventTime { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be in the range from {1} to {2}.")]
        public int? TransferId { get; set; }
        public Transfer? Transfer { get; set; }

    }
}
