using DataAccess.Models.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAccess.Models.Models
{
    public class User
    {
        public int Id { get; set; }

        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? FirstName { get; set; }

        [MinLength(Constants.NameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.NameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.MinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string Username { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [EmailAddress(ErrorMessage = Constants.EmailFieldErrorMessage)]
        public string Email { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Password]
        public byte[] Password { get; set; }
        public byte[]? PasswordKey { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [PhoneNumber(ErrorMessage = Constants.PhoneNumberFieldErroMessage)]
        [StringLength(Constants.PhoneNumberLength, ErrorMessage = Constants.LengthErrorMessage)]
        public string PhoneNumber { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public string? ProfilePhotoFileName { get; set; }

        [NotMapped]
        [DisplayName(Constants.ImageFileFieldErrorMessage)]
        public IFormFile ImageFile { get; set; }
        [JsonIgnore]
        public Account? Account { get; set; }
        public int? AccountId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVerified { get; set; }

        [DateOfBirth(ErrorMessage = Constants.BirthDateErrorMessage)]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(Constants.AddressMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? Address { get; set; }

        [MaxLength(Constants.CountryMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? Country { get; set; }

        [MaxLength(Constants.CityMaxLength, ErrorMessage = Constants.MaxLengthErrorMessage)]
        public string? City { get; set; }
    }
}
