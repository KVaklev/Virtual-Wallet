using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetAccountDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public string DateCreated { get; set; }

        [Required]
        public decimal Balance { get; set; }
    }
}

