using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetTransactionDto
    {
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
    }
}
