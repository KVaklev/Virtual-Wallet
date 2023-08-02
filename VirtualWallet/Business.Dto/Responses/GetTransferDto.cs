using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetTransferDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string TransferType { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}
