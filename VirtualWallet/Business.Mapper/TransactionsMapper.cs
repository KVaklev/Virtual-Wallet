using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class TransactionsMapper : Profile
    {
        public TransactionsMapper()
        {
            //DTO
            CreateMap<CreateTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecepient.User.Username, t => t.MapFrom(t => t.RecepientUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ReverseMap();

            CreateMap<GetTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecepient.User.Username, t => t.MapFrom(t => t.RecipientUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForPath(t => t.Direction, t => t.MapFrom(t => t.Direction))
                .ReverseMap();


        }
    }
}
