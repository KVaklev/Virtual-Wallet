using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class TransactionsMapper : Profile
    {
        public TransactionsMapper()
        {
            //DTO
            CreateMap<CreateTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecipient.User.Username, t => t.MapFrom(t => t.RecepientUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => DirectionType.Out))
                .ForMember(dest => dest.IsExecuted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ReverseMap();

            CreateMap<GetTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecipient.User.Username, t => t.MapFrom(t => t.RecipientUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForPath(t => t.Direction, t => t.MapFrom(t => t.Direction))
                .ReverseMap();


        }
    }
}
