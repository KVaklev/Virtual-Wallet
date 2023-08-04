using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Business.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Username { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Password]
        public string Password { get; set; }
    }
}
