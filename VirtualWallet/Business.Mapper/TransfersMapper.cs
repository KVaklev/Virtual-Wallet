using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class TransfersMapper : Profile
    {
        public TransfersMapper()
        {
            //DTO
            CreateMap<CreateTransferDto, Transfer>()
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForPath(t => t.Card.CardNumber, t => t.MapFrom(t => t.CardNumber))
                .ForPath(t => t.TransferType, t => t.MapFrom(t => t.TransferType))
                .ReverseMap();



            CreateMap<GetTransferDto, Transfer>()
                .ForPath(t=>t.Account.User.Username, t=>t.MapFrom(t=>t.Username))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForPath(t => t.Card.CardNumber, t => t.MapFrom(t => t.CardNumber))
                .ForPath(t => t.TransferType, t => t.MapFrom(t => t.TransferType))
                .ReverseMap();



        }
    }
}
