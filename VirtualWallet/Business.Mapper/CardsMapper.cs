using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class CardsMapper : Profile
    {
        public CardsMapper()
        {
            //DTO
            CreateMap<GetCardDto, Card>()
               .ForPath(c => c.Account.User.Username, c => c.MapFrom(c => c.Username))
               .ForPath(c => c.CardType, c => c.MapFrom(c => c.CardType))
               .ReverseMap();

            CreateMap<CreateCardDto, Card>()
                .ForPath(c => c.Account.User.Username, c => c.MapFrom(c => c.AccountUsername))
                .ForPath(c => c.CardType, c => c.MapFrom(c => c.CardType))
                .ForPath(c => c.Currency.CurrencyCode, c =>c.MapFrom(c =>c.CurrencyCode))
                .ReverseMap();

            CreateMap<UpdateCardDto, Card>()
               .ForPath(c => c.CardType, c => c.MapFrom(c => c.CardType))
               .ForPath(c => c.Currency.CurrencyCode, c => c.MapFrom(c => c.CurrencyCode))
               .ReverseMap();


        }
    }
}
