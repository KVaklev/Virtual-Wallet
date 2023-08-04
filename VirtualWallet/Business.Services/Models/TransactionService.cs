
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
using Business.DTOs;
using Business.Mappers;
using DataAccess.Repositories.Helpers;
using DataAccess.Repositories.Models;

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
            var transaction = await this.transactionRepository.GetByIdAsync(id);
            if (!await Security.IsTransactionSenderAsync(transaction, loggedUser.Id) || !loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            var transactionDto = this.mapper.Map<GetTransactionDto>(transaction);
            result.Data = transactionDto;

            return result;
        }

        public async Task<PaginatedList<Transaction>> FilterByAsync(
            TransactionQueryParameters filterParameters,
            User loggedUser)
        {
            var result = await this.transactionRepository.FilterByAsync(filterParameters, loggedUser.Username);
            return result;//todo paginated
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
            var currency = await currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode);
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
            var currency = await currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode);

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

            var transactionIn = await CreateInTransactionAsync(transactionOut);
            UpdateAccountsBalances(transactionOut, transactionIn);
            await AddTransactionToHistoryAsync(transactionOut);
            await AddTransactionToHistoryAsync(transactionIn);

            transactionOut.IsExecuted = true;
            transactionOut.Date = DateTime.UtcNow;
            await transactionRepository.SaveChangesAsync();

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
            var transactionIn = await TransactionsMapper.MapCreateDtoToTransactionInAsync(transaction, amount);
            await this.transactionRepository.CreateTransactionAsync(transactionIn);
            return transactionIn;
        }

        private async Task<decimal> GetCorrectAmountAsync(
            string transactionCurrencyCode,
            string accountCurrencyCode,
            decimal amount)
        {
            if (transactionCurrencyCode != accountCurrencyCode)
            {
                amount = await this.exchangeRateService
                    .ExchangeAsync(amount, transactionCurrencyCode, accountCurrencyCode);
            }
            return amount;
        }

        private async void UpdateAccountsBalances(Transaction transactionOut, Transaction transactionIn)
        {
            var amountSender = await GetCorrectAmountAsync(
                transactionOut.Currency.CurrencyCode,
                transactionOut.AccountSender.Currency.CurrencyCode,
                transactionOut.Amount);

            await this.accountService.DecreaseBalanceAsync(transactionOut.AccountSenderId, amountSender, transactionOut.AccountSender.User);
            await this.accountService.IncreaseBalanceAsync(transactionIn.AccountRecepientId, transactionIn.Amount, transactionIn.AccountRecipient.User);
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
