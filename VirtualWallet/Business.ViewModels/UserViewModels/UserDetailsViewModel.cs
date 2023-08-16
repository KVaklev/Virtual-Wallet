using Business.DTOs.Responses;

namespace Business.ViewModels.UserViewModels
{
    public class UserDetailsViewModel
    {
        public GetUserDto User { get; set; }
        public int? Cards { get; set; }
    }
}
