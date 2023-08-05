using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetTransactionDto
    {
        [Required]
        public string SenderUsername { get; set; }

        [Required]
        public string RecipientUsername { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Direction { get; set; }

        [Required]
        public bool IsExecuted { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
