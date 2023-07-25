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
            CreateMap<GetUserDto, User>();
            CreateMap<CreateUserDto, User>();
            CreateMap<User, GetUserDto>();
            CreateMap<User, CreateUserDto>();
            CreateMap<User, UpdateUserDto>();
            CreateMap<UpdateUserDto, User>();

        }
    }
}
