using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class UpdateUserDto
    {
        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? FirstName { get; set; }

        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = Constants.EmailFieldErrorMessage)]
        public string? Email { get; set; }

        [PhoneNumber(ErrorMessage = Constants.PhoneNumberFieldErroMessage)]
        [StringLength(Constants.PhoneNumberLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string? PhoneNumber { get; set; }

        [Password]
        public string? Password { get; set; }

        public bool? IsAdmin { get; set; }

        public bool? IsBlocked { get; set; }
    }
}
