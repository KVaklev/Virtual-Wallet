﻿using AutoMapper;
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
        }
    }
}
