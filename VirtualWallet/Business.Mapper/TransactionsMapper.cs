using AutoMapper;
using Business.Dto;
using Business.DTOs;
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
                .ForPath(t => t.Currency.Abbreviation, t => t.MapFrom(t => t.Abbreviation))
                .ReverseMap();

            CreateMap<GetTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecepient.User.Username, t => t.MapFrom(t => t.RecipientUsername))
                .ForPath(t => t.Currency.Abbreviation, t => t.MapFrom(t => t.Abbreviation))
                .ForPath(t => t.Direction, t => t.MapFrom(t => t.Direction))
                .ReverseMap();


        }
    }
}
