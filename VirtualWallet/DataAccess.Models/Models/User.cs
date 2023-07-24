using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.Models
{
    public class User
    {
        public int Id { get; set; }

        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(32, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string? FirstName { get; set; }

        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(32, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(20, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [Password]
        public string Password { get; set; }

        [Required(ErrorMessage = "The {0} field is required")]
        [PhoneNumber(ErrorMessage = "The phone number must contain only digits.")]
        [StringLength(10, ErrorMessage = "The {0} must be exactly {1} characters long.")]
        public string PhoneNumber { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }

        public string? ProfilePhotoPath { get; set; }
        public string? ProfilePhotoFileName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ImageFile { get; set; }
        public Account? Account { get; set; }
        public int? AccountId { get; set; }
    }
}
