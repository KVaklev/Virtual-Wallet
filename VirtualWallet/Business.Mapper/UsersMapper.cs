using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.ViewModels.UserViewModels;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class UsersMapper : Profile
    {
        public UsersMapper()
        {
            //DTO
            CreateMap<GetCreatedUserDto, User>()
                .ForPath(u => u.Account.Currency.CurrencyCode, u => u.MapFrom(u => u.CurrencyCode))
                .ForPath(u => u.Account.Balance, u => u.MapFrom(u => u.Balance))
                .ReverseMap();
            CreateMap<GetUpdatedUserDto, User>();
            CreateMap<User, GetUpdatedUserDto>();
            CreateMap<CreateUserModel, User>();
            CreateMap<User, CreateUserModel>();
            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<GetUserDto, User>();
            

        }

        //public static async Task<UpdateUserDto> MapUserDetailsViewModelToUpdateUserDtoAsync(UserDetailsViewModel userDetailsViewModel, UpdateUserDto updateUserDto)
        //{
        //    updateUserDto.FirstName = userDetailsViewModel.User.FirstName;
        //    updateUserDto.LastName = userDetailsViewModel.User.LastName;
        //    updateUserDto.Email = userDetailsViewModel.User.Email;
        //    updateUserDto.PhoneNumber = userDetailsViewModel.User.PhoneNumber;
        //    updateUserDto
        //}
        public static async Task<User> MapCreateDtoToUserAsync(CreateUserModel createUserDto)
        {
            User newUser = new User();
            newUser.Username = createUserDto.Username;
            newUser.Email = createUserDto.Email;
            newUser.PhoneNumber = createUserDto.PhoneNumber;
            return newUser;
        }

        public static async Task<User> MapUpdateDtoToUserAsync(User userToUpdate, UpdateUserDto updateUserDto)
        {
            userToUpdate.Email = updateUserDto.Email ?? userToUpdate.Email;
            userToUpdate.PhoneNumber = updateUserDto.PhoneNumber ?? userToUpdate.PhoneNumber;
            userToUpdate.FirstName = updateUserDto.FirstName ?? userToUpdate.FirstName;
            userToUpdate.LastName = updateUserDto.LastName ?? userToUpdate.LastName;

            return userToUpdate;
        }
    }
}
