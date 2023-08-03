using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Contracts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.DTOs;
using System.Diagnostics;

namespace Business.Services.Models
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IHistoryRepository historyRepository;
        private readonly ApplicationContext context;
        private readonly IAccountRepository accountRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IMapper mapper;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IAccountService accountService;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IHistoryRepository historyRepository,
            ApplicationContext context,
            IAccountRepository accountRepository,
            ICurrencyRepository currencyRepository,
            IMapper mapper,
            IExchangeRateService exchangeRateService,
            IAccountService accountService
            )
        {
            this.transactionRepository = transactionRepository;
            this.historyRepository = historyRepository;
            this.context = context;
            this.accountRepository = accountRepository;
            this.currencyRepository = currencyRepository;
            this.mapper = mapper;
            this.exchangeRateService = exchangeRateService;
            this.accountService = accountService;
        }
        public async Task<Response<GetTransactionDto>> GetByIdAsync(int id, User loggedUser)
        {
            var result = new Response<GetTransactionDto>();
            if (!await IsTransactionSenderAsync(id, loggedUser.Id) || !loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            var transaction = await this.transactionRepository.GetByIdAsync(id);
            var transactionDto = this.mapper.Map<GetTransactionDto>(transaction);
            result.Data = transactionDto;

            return result;
        }

        public async Task<PaginatedList<Transaction>> FilterByAsync(
            TransactionQueryParameters filterParameters, 
            User loggedUser)
        {
            var result = await this.transactionRepository.FilterByAsync(filterParameters, loggedUser.Username);
            return result;
        }

        public async Task<Response<GetTransactionDto>> CreateOutTransactionAsync(CreateTransactionDto transactionDto, User loggedUser)
        {
            var result = new Response<GetTransactionDto>();
            if (loggedUser.IsBlocked)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionBlockedErrorMessage;
                return result;
            }
            
            var transaction = await MapDtoТоTransactionAsync(transactionDto, loggedUser);
            if (!await Security.HasEnoughBalanceAsync(transaction.AccountSender, transaction.Amount))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }

            var newTransaction = await this.transactionRepository.CreateOutTransactionAsync(transaction);
            result.Data = this.mapper.Map<GetTransactionDto>(newTransaction);

            return result;
        }

        public async Task<Response<GetTransactionDto>> UpdateAsync(int id, User loggedUser, CreateTransactionDto transactionDto)
        {
            var result = new Response<GetTransactionDto>();
            if (!await IsTransactionSenderAsync(id, loggedUser.Id))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }

            var transactionToUpdate = await this.transactionRepository.GetByIdAsync(id);
            if (!await CanModifyTransactionAsync(transactionToUpdate))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }
            var newTransaction = await MapDtoТоTransactionAsync(transactionDto, loggedUser);
            if (!await Security.HasEnoughBalanceAsync(newTransaction.AccountSender, newTransaction.Amount))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }

            var updatedTransacion = await this.transactionRepository.UpdateAsync(transactionToUpdate, newTransaction);
            result.Data = this.mapper.Map<GetTransactionDto>(updatedTransacion);
            return result;
        }

        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();
            if (!await IsTransactionSenderAsync(id, loggedUser.Id))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }

            var transaction = await this.transactionRepository.GetByIdAsync(id);
            if (!await CanModifyTransactionAsync(transaction))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }
            result.Message = Constants.ModifyTransactionDeleteMessage;
            result.Data = await this.transactionRepository.DeleteAsync(transaction);
            return result;
        }


        public async Task<Response<bool>> ExecuteAsync(int transactionId, User loggedUser)
        {
            var result = new Response<bool>();
            if (!await IsTransactionSenderAsync(transactionId, loggedUser.Id))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }

            var transactionOut = await this.transactionRepository.GetByIdAsync(transactionId);
            if(!await CanModifyTransactionAsync(transactionOut))
            { 
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }

            

            var transactionIn = await CreateInTransactionAsync(transactionOut);
            UpdateAccountsBalances(transactionOut, transactionIn);
            await AddTransactionToHistoryAsync(transactionOut);
            await AddTransactionToHistoryAsync(transactionIn);

            result.Message = Constants.ModifyTransactionExecuteMessage;
            result.Data = transactionOut.IsExecuted;
            return result;
        }

        private async Task<Transaction> CreateInTransactionAsync(Transaction transaction)
        {
            var amount = await GetCorrectAmountAsync(
                transaction.Currency.CurrencyCode, 
                transaction.AccountRecipient.Currency.CurrencyCode, 
                transaction.Amount);
            var transactionIn = await this.transactionRepository.CreateInTransactionAsync(transaction, amount);
            return transactionIn;
        }

        private async Task<decimal> GetCorrectAmountAsync(
            string transactionCurrencyCode, 
            string accountCurrencyCode, 
            decimal amount)
        {
            if (transactionCurrencyCode!= accountCurrencyCode)
            {
                amount = await this.exchangeRateService
                    .ExchangeAsync(amount, transactionCurrencyCode, accountCurrencyCode);
            }
            return amount;
        }

        private async void UpdateAccountsBalances(Transaction transactionOut, Transaction transactionIn)
        {
             var amountSender =await GetCorrectAmountAsync(
                 transactionOut.Currency.CurrencyCode,
                 transactionOut.AccountSender.Currency.CurrencyCode,
                 transactionOut.Amount );
           
             await this.accountService.DecreaseBalanceAsync(transactionOut.AccountSenderId, amountSender, transactionOut.AccountSender.User);
             await this.accountService.IncreaseBalanceAsync(transactionIn.AccountRecepientId, transactionIn.Amount, transactionIn.AccountRecipient.User);
        }

        private async Task<bool> AddTransactionToHistoryAsync(Transaction transaction)
        {

            int historyCount = await this.context.History.CountAsync();
                               await this.historyRepository.CreateWithTransactionAsync(transaction);
            int newHistoryCount = await this.context.History.CountAsync();

            if (newHistoryCount == historyCount + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // todo - move to mappers
        private async Task<Transaction> MapDtoТоTransactionAsync(CreateTransactionDto transactionDto, User user)
        {
            var transaction = this.mapper.Map<Transaction>(transactionDto);
            transaction.AccountSenderId = (int)user.AccountId;
            transaction.AccountSender = user.Account;
            transaction.AccountRecipient = await this.accountRepository.GetByUsernameAsync(transactionDto.RecepientUsername);
            transaction.Currency = await this.currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode);
            transaction.AccountRecepientId = (await this.accountRepository.GetByUsernameAsync(transactionDto.RecepientUsername)).Id;
            transaction.CurrencyId = (await this.currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode)).Id;
            transaction.Direction = DirectionType.Out;
            return transaction;
        }

        private async Task<bool> IsTransactionSenderAsync(int id, int userId)
        {
            bool isTransactionSender = true;
            Transaction transactionToUpdate = await this.transactionRepository.GetByIdAsync(id);

            if (transactionToUpdate.AccountSender.User.Id != userId)
            {
                isTransactionSender = false;
            }
            return isTransactionSender;
        }

        private async Task<bool> CanModifyTransactionAsync(Transaction transaction)
        {
                var canExecuteTransaction = true;
                        if (transaction.IsExecuted
                                || transaction.Direction == DirectionType.In
                                || transaction.IsDeleted)
                                {
                                    canExecuteTransaction = false;
                                }
                return canExecuteTransaction;
        }
    }
}
