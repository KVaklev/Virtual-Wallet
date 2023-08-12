using Business.DTOs.Requests;
using Microsoft.AspNetCore.Http;

namespace Business.ViewModels.UserViewModels
{
    public class UserUpdateProfileViewModel
    {
        public UpdateUserDto UpdateUserDto { get; set; }

        public UserDetailsViewModel DetailsViewModel { get; set; }

    }
}
