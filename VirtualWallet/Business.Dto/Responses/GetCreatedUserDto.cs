using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetCreatedUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public string CurrencyCode { get; set; }
    }
}
