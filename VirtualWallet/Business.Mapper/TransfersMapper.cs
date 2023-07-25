using AutoMapper;
using Business.Dto;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class TransfersMapper : Profile
    {
        public TransfersMapper()
        {
            //DTO
            CreateMap<TransferDto, Transfer>();
            CreateMap<Transfer, TransferDto>();

        }
    }
}
