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
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public decimal? AccountBalance { get; set; }
        public bool ConfirmedRegistration { get; set; }

    }
}
