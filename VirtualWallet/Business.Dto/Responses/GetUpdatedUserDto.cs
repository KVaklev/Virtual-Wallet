using Microsoft.AspNetCore.Http;


namespace Business.DTOs.Responses
{
    public class GetUpdatedUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
