using AutoMapper;
using Business.DTOs;
using Business.DTOs.Responses;
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
            .ForMember(a => a.CurrencyCode, c => c.MapFrom(a => a.Currency.CurrencyCode));

        }
    }
}
