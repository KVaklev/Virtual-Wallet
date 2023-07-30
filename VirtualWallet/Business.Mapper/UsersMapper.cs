using AutoMapper;
using Business.Dto;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class UsersMapper : Profile
    {
        public UsersMapper()
        {
            //DTO
            CreateMap<GetUserDto, User>()
                .ForPath(u => u.Account.Currency.Abbreviation, u => u.MapFrom(u => u.Abbreviation))
                .ForPath(u => u.Account.Balance, u => u.MapFrom(u => u.Balance))
                .ReverseMap(); 
            CreateMap<CreateUserDto, User>();
            // CreateMap<User, GetUserDto>(); - TODO - check is this necessary
            CreateMap<User, CreateUserDto>();
            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>();

        }
    }
}
