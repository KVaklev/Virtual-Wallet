﻿using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class TransactionsMapper : Profile
    {
        public TransactionsMapper()
        {
            //DTO
            CreateMap<CreateTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecipient.User.Username, t => t.MapFrom(t => t.RecepientUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => DirectionType.Out))
                .ReverseMap();

            CreateMap<GetTransactionDto, Transaction>()
                .ForPath(t => t.AccountRecipient.User.Username, t => t.MapFrom(t => t.RecipientUsername))
                .ForPath(t => t.Currency.CurrencyCode, t => t.MapFrom(t => t.CurrencyCode))
                .ForPath(t => t.Direction, t => t.MapFrom(t => t.Direction))
                .ReverseMap();
        }

            public static async Task<Transaction> MapCreateDtoToTransactionInAsync(Transaction transactionOut, decimal amount)
            {
                var transactionIn = new Transaction();
                transactionIn.AccountRecepientId = transactionOut.AccountRecepientId;
                transactionIn.AccountSenderId = transactionOut.AccountSenderId;
                transactionIn.Amount = amount;
                transactionIn.CurrencyId = (int)transactionOut.AccountRecipient.CurrencyId;
                transactionIn.Direction = DirectionType.In;
                transactionIn.Date = DateTime.UtcNow;
            transactionIn.Description = transactionOut.Description;
                transactionIn.IsExecuted = true;
                return transactionIn;
            }

        public static async Task<Transaction> MapUpdateDtoToTransactionAsync(
            Transaction transactionToUpdate, 
            Transaction transaction)
        {
            transactionToUpdate.AccountRecepientId = transaction.AccountRecepientId;
            transactionToUpdate.Amount = transaction.Amount;
            transactionToUpdate.CurrencyId = transaction.CurrencyId;
            transactionToUpdate.Description = transaction.Description;
            transactionToUpdate.Date = DateTime.UtcNow;
            return transactionToUpdate;
        }

        public static async Task<Transaction> MapDtoТоTransactionAsync(
            CreateTransactionDto transactionDto, 
            User user, 
            Account account, 
            Currency currency)
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
            return transaction;
        }

    }
}
