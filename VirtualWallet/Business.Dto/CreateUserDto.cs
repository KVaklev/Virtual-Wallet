using DataAccess.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.Dto
{
    public class CreateUserDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(20, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please provide a valid email.")]
        public string Email { get; set; }

        [Required]
        [PhoneNumber(ErrorMessage = "The phone number must contain only digits.")]
        [StringLength(10, ErrorMessage = "The {0} must be {1} characters long.")]
        public string PhoneNumber { get; set; }

        [Required]
        [Password]
        public string Password { get; set; }
    }
}
