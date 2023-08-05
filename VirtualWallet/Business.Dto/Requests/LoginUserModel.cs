using System.ComponentModel.DataAnnotations;

namespace Business.ViewModels
{
    public class LoginUserModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
