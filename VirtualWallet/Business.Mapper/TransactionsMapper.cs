using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using Org.BouncyCastle.Cms;

namespace Business.Mappers
{
    public class TransactionsMapper : Profile
    {
        public TransactionsMapper()
        {
            //DTO
            CreateMap<CreateTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecipient.User.Username, t => t.MapFrom(t => t.RecipientUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => DirectionType.Out))
                .ReverseMap();

            CreateMap<GetTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecipient.User.Username, t => t.MapFrom(t => t.RecipientUsername))
                .ForPath(t => t.AccountSender.User.Username, t => t.MapFrom(t => t.SenderUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForPath(t => t.Direction, t => t.MapFrom(t => t.Direction))
                .ReverseMap();
        }

        public static async Task<Transaction> MapCreateDtoToTransactionInAsync(
            Transaction transactionOut, 
            decimal amount)
        {
            var transactionIn = new Transaction();
            transactionIn.AccountRecepientId = transactionOut.AccountRecepientId;
            transactionIn.AccountSenderId = transactionOut.AccountSenderId;
            transactionIn.Amount = amount;
            transactionIn.CurrencyId = (int)transactionOut.AccountRecipient.CurrencyId;
            transactionIn.Direction = DirectionType.In;
            transactionIn.Date = DateTime.UtcNow;
            transactionIn.Description = transactionOut.Description;
            transactionIn.IsConfirmed = true;
            transactionIn.ExchangeRate=transactionOut.ExchangeRate;
            return transactionIn;
        }

        public static async Task<Transaction> MapUpdateDtoToTransactionAsync(
            Transaction transactionToUpdate, 
            CreateTransactionDto transactionDto,
            Account recipient,
            Currency currency,
            decimal exchangeRate)
        {
            transactionToUpdate.AccountRecepientId = recipient.Id;
            transactionToUpdate.AccountRecipient = recipient;
            transactionToUpdate.Amount = transactionDto.Amount;
            transactionToUpdate.CurrencyId = currency.Id;
            transactionToUpdate.Currency = currency;
            transactionToUpdate.Description = transactionDto.Description;
            transactionToUpdate.Date = DateTime.UtcNow;
            transactionToUpdate.AmountExchange = transactionDto.Amount*exchangeRate;
            transactionToUpdate.ExchangeRate = exchangeRate;
            return transactionToUpdate;
        }

        public static async Task<Transaction> MapDtoТоTransactionAsync(
            CreateTransactionDto transactionDto, 
            User user, 
            Account account, 
            Currency currency,
            decimal exchangeRate)
        {
            var transaction = new Transaction();
            transaction.AccountSenderId = (int)user.AccountId;
            transaction.AccountSender = user.Account;
            transaction.AccountRecipient = account;
            transaction.Amount = transactionDto.Amount;
            transaction.Currency = currency;
            transaction.AccountRecepientId = account.Id;
            transaction.CurrencyId = currency.Id;
            transaction.Direction = DirectionType.Out;
            transaction.Description = transactionDto.Description;
            transaction.Date = DateTime.UtcNow;
            transaction.AmountExchange = transactionDto.Amount*exchangeRate;
            transaction.ExchangeRate = exchangeRate;
            return transaction;
        }

        public static async Task<CreateTransactionDto> MapGetDtoToCreateDto(GetTransactionDto getTransactionDto)
        {
            var createTransactionDto = new CreateTransactionDto();
            createTransactionDto.CurrencyCode = getTransactionDto.CurrencyCode;
            createTransactionDto.Amount = getTransactionDto.Amount;
            createTransactionDto.Description = getTransactionDto.Description;
            createTransactionDto.RecipientUsername = getTransactionDto.RecipientUsername;

            return createTransactionDto;
        }
    }
}
