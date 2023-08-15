using System.ComponentModel.DataAnnotations;

namespace Business.DTOs.Requests
{
    public class LoginUserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
