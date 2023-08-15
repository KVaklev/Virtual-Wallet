using AutoMapper;
using Business.DTOs.Requests;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class CurrenciesMapper : Profile
    {
        public CurrenciesMapper()
        {
            //DTO
            CreateMap<CreateCurrencyDto, Currency>();
            CreateMap<Currency, CreateCurrencyDto>();
        }
        
    }
}
