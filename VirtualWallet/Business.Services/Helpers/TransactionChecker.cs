
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.Services.Helpers
{
    public static class TransactionChecker
    {
        public static async Task<Response<GetTransactionDto>> ChecksGetByIdAsync(Transaction transaction, User loggedUser)
        {
            var result = new Response<GetTransactionDto>();
            
            if (transaction == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!await Security.IsTransactionSenderAsync(transaction, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            return result;
        }

        public static async Task<Response<GetTransactionDto>> ChecksCreateOutTransactionAsync(
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


        public static async Task<Response<GetTransactionDto>> ChecksUpdateAsync(
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
            if (!await Security.IsTransactionSenderAsync(transactionToUpdate, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            if (!await Security.CanModifyTransactionAsync(transactionToUpdate))
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
        public static async Task<Response<bool>> ChecksDeleteAsync(Transaction transaction, User loggedUser)
        {
            var result = new Response<bool>();
            if (transaction == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!await Security.IsTransactionSenderAsync(transaction, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }

            if (!await Security.CanModifyTransactionAsync(transaction))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }
            return result;
        }

        public static async Task<Response<bool>> ChecksConfirmAsync(Transaction transaction, User loggedUser)
        {
            var result = new Response<bool>();
            if (transaction == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!await Security.IsTransactionSenderAsync(transaction, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            if (!await Security.CanModifyTransactionAsync(transaction))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }
            return result;
        }
    }
}
