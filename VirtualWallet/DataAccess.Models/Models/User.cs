using DataAccess.Models.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.Models
{
    public class User
    {
        public int Id { get; set; }

        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.NameMinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.NameMaxLengthErrorMessage)]
        public string? FirstName { get; set; }

        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.NameMinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.NameMaxLengthErrorMessage)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.UsernameMinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.UsernameMaxLengthErrorMessage)]
        public string Username { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [EmailAddress(ErrorMessage = Constants.EmailFieldErrorMessage)]
        public string Email { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Password]
        public string Password { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [PhoneNumber(ErrorMessage = Constants.PhoneNumberFieldErroMessage)]
        [StringLength(Constants.PhoneNumberLength, ErrorMessage = Constants.PhoneNumberLengthErrorMessage)]
        public string PhoneNumber { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }

        public string? ProfilePhotoPath { get; set; }
        public string? ProfilePhotoFileName { get; set; }

        [NotMapped]
        [DisplayName(Constants.ImageFileFieldErrorMessage)]
        public IFormFile ImageFile { get; set; }
        public Account? Account { get; set; }
        public int? AccountId { get; set; }
        public bool IsDeleted { get; set; }

    }
}
