using Microsoft.AspNetCore.Http;

namespace Business.DTOs.Responses
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool Admin { get; set; }
        public bool Blocked { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public decimal? AccountBalance { get; set; }
        public bool ConfirmedRegistration { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public string? ProfilePhotoFileName { get; set; }

    }
}
