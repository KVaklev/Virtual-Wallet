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
                .ForPath(c => c.Account.User.Username, c => c.MapFrom(c => c.Username));
            CreateMap<Card, GetCardDto>()
                .ForPath(c => c.Username, c => c.MapFrom(c => c.Account.User.Username));
            CreateMap<GetCardDto, Card>()
               .ForMember(c => c.CardType, c => c.MapFrom(c => c.CardType));
            CreateMap<Card, GetCardDto>()
              .ForMember(c => c.CardType, c => c.MapFrom(c => c.CardType));
        }
    }
}
