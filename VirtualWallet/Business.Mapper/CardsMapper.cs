using AutoMapper;
using Business.Dto;
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
                .ForPath(c => c.Currency.Abbreviation, c =>c.MapFrom(c =>c.Currency))
                .ReverseMap();

            CreateMap<Card, CreateCardDto>()
               .ForPath(c => c.AccountUsername, c => c.MapFrom(c => c.Account.User.Username))
               .ForPath(c => c.CardType, c => c.MapFrom(c => c.CardType))
               .ForPath(c => c.Currency, c => c.MapFrom(c => c.Currency.Abbreviation))
               .ReverseMap();
        }
    }
}
