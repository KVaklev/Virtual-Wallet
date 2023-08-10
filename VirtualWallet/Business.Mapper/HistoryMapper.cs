﻿using AutoMapper;
using Business.DTOs.Responses;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;

namespace Business.Mappers
{
    public class HistoryMapper : Profile
    {
        public static async Task<History> MapCreateWithTransferAsync(Transfer transfer)
        {
            var history = new History();

            history.EventTime = DateTime.Now;
            history.TransferId = transfer.Id;
            history.Transfer = transfer;
            history.NameOperation = NameOperation.Transfer;
            history.AccountId = transfer.AccountId;
            return history;
        }

        public static async Task<History> MapCreateWithTransactionAsync(Transaction transaction)
        {
            var history = new History();

            history.EventTime = DateTime.Now;
            history.TransactionId = transaction.Id;
            history.Transaction = transaction;
            history.NameOperation = NameOperation.Transaction;

            if (transaction.Direction == DirectionType.Out)
            {
                history.AccountId = transaction.AccountSenderId;
            }
            else
            {
                history.AccountId = transaction.AccountRecepientId;
            }
            return history;
        }

            public static GetHistoryDto MapHistoryToDtoAsync(History history)
        {
            var historyDto = new GetHistoryDto();
            historyDto.EventTime = history.EventTime.ToString();
            historyDto.NameOperation = history.NameOperation.ToString();

            if (history.TransactionId != null)
            {
                historyDto.From = history.Account.User.Username;
               // historyDto.To = transaction.AccountRecipient.User.Username;
                historyDto.Amount = history.Transaction.Amount;
                historyDto.CurrencyCode = history.Transaction.Currency.CurrencyCode;
                historyDto.Direction = history.Transaction.Direction.ToString();
            }
            else
            {
                historyDto.Amount = history.Transfer.Amount;
                historyDto.CurrencyCode = history.Transfer.Currency.CurrencyCode;
                historyDto.Direction = history.Transfer.TransferType.ToString();

                if (history.Transfer.TransferType == TransferDirection.Deposit)
                {
                    historyDto.From = history.Transfer.Card.CardNumber;
                    historyDto.To = history.Account.User.Username;
                }
                else
                {
                    historyDto.From = history.Account.User.Username;
                    historyDto.To = history.Transfer.Card.CardNumber;
                }
            }
            return historyDto;
        }
    }
}
