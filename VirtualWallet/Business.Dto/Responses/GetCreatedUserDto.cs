using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetCreatedUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
    }
}
