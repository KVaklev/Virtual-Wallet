using AutoMapper;
using Business.Dto;
using Business.DTOs;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class AccountsMapper : Profile
    {
        public AccountsMapper()
        {
            //DTO
            CreateMap<GetAccountDto, Account>();

            CreateMap<Account, GetAccountDto>()
            .ForMember(u => u.Username, t => t.MapFrom(a => a.User.Username))
            .ForMember(a => a.Abbreviation, c => c.MapFrom(a => a.Currency.Abbreviation));

            CreateMap<Account, CreateAccountDto>()
            .ForMember(u => u.Abbreviation, c => c.MapFrom(a => a.Currency.Abbreviation));
        }
    }
}
