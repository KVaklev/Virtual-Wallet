using System.ComponentModel.DataAnnotations;

namespace Business.Dto
{
    public class GetUserDto
    {
        [Required]
        public string UserName { get; set;}

        [Required]
        public string Email { get; set;}

        [Required]
        public string PhoneNumber { get; set;}

        [Required]
        public decimal Balance { get; set;}

        [Required]
        public string Abbreviation { get; set;}
    }
}
