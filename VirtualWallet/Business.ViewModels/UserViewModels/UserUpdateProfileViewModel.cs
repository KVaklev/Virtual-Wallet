using Business.DTOs.Requests;

namespace Business.ViewModels.UserViewModels
{
    public class UserUpdateProfileViewModel
    {
        public UpdateUserDto UpdateUserDto { get; set; }

        public UserDetailsViewModel DetailsViewModel { get; set; }
    }
}
