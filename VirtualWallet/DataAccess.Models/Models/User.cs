using DataAccess.Models.Contracts;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.Models
{
    public class User : IUser
    {
        public int Id { get; set; }

        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(32, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string FirstName { get; set; }

        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(32, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string LastName { get; set; }
        
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(20, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Please provide a valid email.")]
        public string Email { get; set; }

        [Password]
        public string Password { get; set; }

        [Range(10, 10, ErrorMessage = "Phone number must be exactly {0} digits long.")]
        public int PhoneNumber { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }

        public string ProfilePhotoPath { get; set; }
        public string ProfilePhotoFileName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ImageFile { get; set; }

        public List<Transaction> TransactionsSender { get; set; } = new List<Transaction>();
        public List<Transaction> TransactionsRecipiend { get; set; } = new List<Transaction>();


    }
}
