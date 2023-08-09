using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Enums;
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
                .ForPath(t => t.Account.User.Username, t => t.MapFrom(t => t.Username))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForPath(t => t.Card.CardNumber, t => t.MapFrom(t => t.CardNumber))
                .ForPath(t => t.TransferType, t => t.MapFrom(t => t.TransferType))
                .ReverseMap();
        }

        public static async Task<Transfer> MapCreateDtoToTransferAsync(
            CreateTransferDto transferDto,
            User user, Card card,
            Currency currency,
            TransferDirection transferDirection)
        {
            var transfer = new Transfer();
            transfer.Amount = transferDto.Amount;
            transfer.AccountId = (int)user.AccountId;
            transfer.Account = user.Account;
            transfer.Currency = currency;
            transfer.Card = card;
            transfer.CardId = (int)card.Id;
            transfer.CurrencyId = transfer.Currency.Id;
            transfer.TransferType = transferDirection;

            return transfer;
        }

        public static async Task<Transfer> MapUpdateDtoToTransferAsync(
            Transfer transfer,
            UpdateTransferDto transferDto,
            Card card,
            Currency currency)
        {

            transfer.Amount = transferDto.Amount;
            transfer.Currency = currency;
            transfer.Card = card;
            transfer.CardId = (int)card.Id;
            transfer.CurrencyId = transfer.Currency.Id;
            return transfer;
        }
    }
}
