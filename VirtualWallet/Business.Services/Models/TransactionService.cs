﻿
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Contracts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Mappers;


namespace Business.Services.Models
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly ApplicationContext context;
        private readonly IAccountRepository accountRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IMapper mapper;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IAccountService accountService;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ApplicationContext context,
            IAccountRepository accountRepository,
            ICurrencyRepository currencyRepository,
            IMapper mapper,
            IExchangeRateService exchangeRateService,
            IAccountService accountService
            )
        {
            this.transactionRepository = transactionRepository;
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
            var transaction = await this.transactionRepository.GetByIdAsync(id);
            if (transaction==null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }
            if (!await Security.IsTransactionSenderAsync(transaction, loggedUser.Id))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            var transactionDto = this.mapper.Map<GetTransactionDto>(transaction);
            result.Data = transactionDto;

            return result;
        }

        public async Task<Response<PaginatedList<GetTransactionDto>>> FilterByAsync(
            TransactionQueryParameters filterParameters,
            User loggedUser)
        {
            var result = new Response<PaginatedList<GetTransactionDto>>();
            var transactions = await this.transactionRepository.FilterByAsync(filterParameters, loggedUser.Username);
            if (transactions.Count==0)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }

            List<GetTransactionDto> transactionDtos = transactions
                    .Select(transaction => mapper.Map<GetTransactionDto>(transaction))
                    .ToList();
            result.Data = new PaginatedList<GetTransactionDto>(transactionDtos,transactions.TotalPages,transactions.PageNumber);
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
            
            var account = await accountRepository.GetByUsernameAsync(transactionDto.RecepientUsername);
            if (account == null) 
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }
            var currency = await currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode);
            if (currency == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }
            var transaction = await TransactionsMapper.MapDtoТоTransactionAsync(transactionDto, loggedUser, account, currency);
            
            if (!await Security.HasEnoughBalanceAsync(transaction.AccountSender, transaction.Amount))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }

            transaction.Date = DateTime.UtcNow;
            var newTransaction = await this.transactionRepository.CreateTransactionAsync(transaction);
            result.Data = this.mapper.Map<GetTransactionDto>(newTransaction);

            return result;
        }

        public async Task<Response<GetTransactionDto>> UpdateAsync(int id, User loggedUser, CreateTransactionDto transactionDto)
        {
            var result = new Response<GetTransactionDto>();
            var transactionToUpdate = await this.transactionRepository.GetByIdAsync(id);
            if (transactionToUpdate==null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }
            if (!await Security.IsTransactionSenderAsync(transactionToUpdate, loggedUser.Id))
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
            if (!await Security.HasEnoughBalanceAsync(loggedUser.Account, loggedUser.Account.Balance))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }
            var account = await accountRepository.GetByUsernameAsync(transactionDto.RecepientUsername);
            if (account == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }
            var currency = await currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode);
            if (currency == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }

            var newTransaction = await TransactionsMapper.MapDtoТоTransactionAsync(transactionDto, loggedUser, account, currency);
            var updatedTransacion = await TransactionsMapper.MapUpdateDtoToTransactionAsync(transactionToUpdate, newTransaction);
            await this.transactionRepository.SaveChangesAsync();
            result.Data = this.mapper.Map<GetTransactionDto>(updatedTransacion);
            return result;
        }
        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();
            var transaction = await this.transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }
            if (!await Security.IsTransactionSenderAsync(transaction, loggedUser.Id))
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
            result.Message = Constants.ModifyTransactionDeleteMessage;
            result.Data = await this.transactionRepository.DeleteAsync(transaction);
            return result;
        }


        public async Task<Response<bool>> ExecuteAsync(int transactionId, User loggedUser)
        {
            var result = new Response<bool>();
            var transactionOut = await this.transactionRepository.GetByIdAsync(transactionId);
            if (transactionOut == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoFoundResulte;
                return result;
            }
            if (!await Security.IsTransactionSenderAsync(transactionOut, loggedUser.Id))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            if (!await Security.CanModifyTransactionAsync(transactionOut))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionNotExecuteErrorMessage;
                return result;
            }

            var transactionInResult = await CreateInTransactionAsync(transactionOut);
            if (!transactionInResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = transactionInResult.Message;
                return result;
            }
            var updareResult =  await UpdateAccountsBalancesAsync(transactionOut, transactionInResult.Data);
            if (!updareResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = updareResult.Message;
                return result;
            }
            await AddTransactionToHistoryAsync(transactionOut);
            await AddTransactionToHistoryAsync(transactionInResult.Data);

            transactionOut.IsExecuted = true;
            transactionOut.Date = DateTime.UtcNow;
            await transactionRepository.SaveChangesAsync();

            result.Message = Constants.ModifyTransactionExecuteMessage;
            result.Data = transactionOut.IsExecuted;
            return result;
        }

        private async Task<Response<Transaction>> CreateInTransactionAsync(Transaction transaction)
        {
            var result = new Response<Transaction>();
            var exchangeAmountResult = new Response<decimal>();
            if (transaction.Currency.CurrencyCode != transaction.AccountRecipient.Currency.CurrencyCode)
            {
              exchangeAmountResult = await this.exchangeRateService
                    .ExchangeAsync(
                  transaction.Amount, 
                  transaction.Currency.CurrencyCode, 
                  transaction.AccountRecipient.Currency.CurrencyCode);
                if (!exchangeAmountResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.Message = exchangeAmountResult.Message;
                    return result;
                } 
            }
            var transactionIn = await TransactionsMapper.MapCreateDtoToTransactionInAsync(transaction, exchangeAmountResult.Data);
            await this.transactionRepository.CreateTransactionAsync(transactionIn);
            result.Data = transactionIn;
            return result;
        }

        private async Task<Response<bool>> UpdateAccountsBalancesAsync(Transaction transactionOut, Transaction transactionIn)
        {
            var result = new Response<bool>();
            var amountSenderResult = await this.exchangeRateService
                    .ExchangeAsync(
                transactionOut.Amount,
                transactionOut.Currency.CurrencyCode,
                transactionOut.AccountSender.Currency.CurrencyCode);
            if (!amountSenderResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = amountSenderResult.Message;
                return result;
            }

            await this.accountService.DecreaseBalanceAsync(transactionOut.AccountSenderId, amountSenderResult.Data, transactionOut.AccountSender.User);
            await this.accountService.IncreaseBalanceAsync(transactionIn.AccountRecepientId, transactionIn.Amount, transactionIn.AccountRecipient.User);
            result.Data = true;

            return result;
        }

        private async Task<bool> AddTransactionToHistoryAsync(Transaction transaction)
        {

            int historyCount = await this.context.History.CountAsync();
            await HistoryMapper.MapCreateWithTransactionAsync(transaction);
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

    }

}
