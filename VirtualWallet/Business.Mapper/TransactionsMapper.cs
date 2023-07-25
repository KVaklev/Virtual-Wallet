using AutoMapper;
using Business.Dto;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class TransactionsMapper : Profile
    {
        public TransactionsMapper()
        {
            //DTO
            CreateMap<Transaction, CreateTransactionDto>()
                .ForMember(r => r.RecepiendUsername, u => u.MapFrom(c => c.AccountRecepient.User.Username))
                .ForMember(c => c.Currency, u => u.MapFrom(a => a.Currency.Аbbreviation));

        }
    }
}
