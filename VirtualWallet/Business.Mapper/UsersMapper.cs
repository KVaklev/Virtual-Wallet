using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
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
            CreateMap<CreateUserDto, User>();
            // CreateMap<User, GetUserDto>(); - TODO - check is this necessary
            CreateMap<User, CreateUserDto>();
            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>();

        }

        public static async Task<User> MapCreateDtoToUserAsync(CreateUserDto createUserDto)
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
