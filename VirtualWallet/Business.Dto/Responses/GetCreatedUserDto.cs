using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Responses
{
    public class GetCreatedUserDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }

    }
}
