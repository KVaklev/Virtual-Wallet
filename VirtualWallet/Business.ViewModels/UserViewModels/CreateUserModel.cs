using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class CreateUserModel
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Username { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [EmailAddress(ErrorMessage = Constants.EmailFieldErrorMessage)]
        public string Email { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [PhoneNumber(ErrorMessage = Constants.PhoneNumberFieldErroMessage)]
        [StringLength(Constants.PhoneNumberLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Password]
        public string Password { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(Constants.CurrencyCodeLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string CurrencyCode { get; set; }
    }
}
