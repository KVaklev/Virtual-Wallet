using DataAccess.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetCardDto
    {
        [Required]
        public string CardNumber { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public string CardHolder { get; set; }

        [Required]
        public string CheckNumber { get; set; }

        [Required]
        public string CardType { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public decimal Balance { get; set; }
    }
}
