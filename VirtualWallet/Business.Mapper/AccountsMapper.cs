using AutoMapper;
using Business.Dto;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class AccountsMapper : Profile
    {
        public AccountsMapper()
        {
            //DTO
            CreateMap<AccountDto, Account>();

            CreateMap<Account, AccountDto>()
            .ForMember(u => u.Username, t => t.MapFrom(a => a.User.Username))
            .ForMember(a => a.Abbreviation, c => c.MapFrom(a => a.Currency.Abbreviation));

        }
    }
}
