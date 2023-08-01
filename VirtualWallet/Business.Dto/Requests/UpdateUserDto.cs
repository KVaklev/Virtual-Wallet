using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class UpdateUserDto
    {
        [EmailAddress(ErrorMessage = Constants.EmailFieldErrorMessage)]
        public string? Email { get; set; }

        [PhoneNumber(ErrorMessage = Constants.PhoneNumberFieldErroMessage)]
        [StringLength(Constants.PhoneNumberLength, ErrorMessage = Constants.PhoneNumberLengthErrorMessage)]
        public string? PhoneNumber { get; set; }

        [Password]
        public string? Password { get; set; }

        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.NameMinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.NameMaxLengthErrorMessage)]
        public string? FirstName { get; set; }

        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.NameMinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.NameMaxLengthErrorMessage)]
        public string? LastName { get; set; }
    }
}
