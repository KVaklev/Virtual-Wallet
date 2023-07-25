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
            CreateMap<GetCardDto, Card>();
            CreateMap<Card, GetCardDto>();

        }
    }
}
