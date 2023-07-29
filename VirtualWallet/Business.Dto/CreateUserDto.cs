using DataAccess.Models.Models;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.Dto
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.UsernameMinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.UsernameMaxLengthErrorMessage)]
        public string Username { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [EmailAddress(ErrorMessage = Constants.EmailFieldErrorMessage)]
        public string Email { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [PhoneNumber(ErrorMessage = Constants.PhoneNumberFieldErroMessage)]
        [StringLength(Constants.PhoneNumberLength, ErrorMessage = Constants.PhoneNumberLengthErrorMessage)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Password]
        public string Password { get; set; }

        [StringLength(3, ErrorMessage = "The {0} must be {1} characters long.")]
        public string Abbreviation { get; set; }
    }
}
