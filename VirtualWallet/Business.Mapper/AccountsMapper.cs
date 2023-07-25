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
            CreateMap<Account, AccountDto>();

        }
    }
}
