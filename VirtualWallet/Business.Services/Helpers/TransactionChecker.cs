﻿using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Services.Contracts;
using DataAccess.Models.Models;

namespace Business.Services.Helpers
{
    public class TransactionChecker : ITransactionCheckerService

    {
        private readonly ISecurityService security;

        public TransactionChecker(ISecurityService security)
        {
            this.security=security;
        }

        public async Task<Response<GetTransactionDto>> ChecksGetByIdAsync(Transaction transaction, User loggedUser)
        {
            var result = new Response<GetTransactionDto>();

            if (transaction == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }

            if (!await security.IsTransactionSenderAsync(transaction, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }

            return result;
        }

        public async Task<Response<GetTransactionDto>> ChecksCreateOutTransactionAsync(
            CreateTransactionDto transactionDto,
            User loggedUser,
            Account recipient,
            Currency currency,
            Response<decimal> exchangeRate)
        {
            var result = new Response<GetTransactionDto>();
            if (loggedUser.IsBlocked)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionBlockedErrorMessage;
                return result;
            }

            if (!await Common.HasEnoughBalanceAsync(loggedUser.Account, transactionDto.Amount))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }

            if (recipient == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }

            if (currency == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }

            if (!exchangeRate.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }

            return result;
        }


        public async Task<Response<GetTransactionDto>> ChecksUpdateAsync(
            Transaction transactionToUpdate,
            User loggedUser,
            CreateTransactionDto transactionDto,
            Account recipient,
            Currency currency,
            Response<decimal> exchangeRate)
        {
            var result = new Response<GetTransactionDto>();

            if (transactionToUpdate == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!await security.IsTransactionSenderAsync(transactionToUpdate, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            if (!await security.CanModifyTransactionAsync(transactionToUpdate))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }
            if (!await Common.HasEnoughBalanceAsync(loggedUser.Account, transactionDto.Amount))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }
            if (recipient == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (currency == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!exchangeRate.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }

            return result;
        }
        public async Task<Response<bool>> ChecksDeleteAsync(Transaction transaction, User loggedUser)
        {
            var result = new Response<bool>();

            if (transaction == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!await security.IsTransactionSenderAsync(transaction, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            if (!await security.CanModifyTransactionAsync(transaction))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }

            return result;
        }

        public async Task<Response<bool>> ChecksConfirmAsync(Transaction transaction, User loggedUser)
        {
            var result = new Response<bool>();

            if (transaction == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!await security.IsTransactionSenderAsync(transaction, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            if (!await security.CanModifyTransactionAsync(transaction))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }

            return result;
        }
    }
}
