using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Contracts;
using Business.Dto;
using AutoMapper;
using Business.DTOs;
using Microsoft.EntityFrameworkCore;

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
        private readonly IUserRepository userRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IHistoryRepository historyRepository,
            ApplicationContext context,
            IAccountRepository accountRepository,
            ICurrencyRepository currencyRepository,
            IMapper mapper,
            IExchangeRateService exchangeRateService,
            IUserRepository userRepository
            )
        {
            this.transactionRepository = transactionRepository;
            this.historyRepository = historyRepository;
            this.context = context;
            this.accountRepository = accountRepository;
            this.currencyRepository = currencyRepository;
            this.mapper = mapper;
            this.exchangeRateService = exchangeRateService;
            this.userRepository = userRepository;
        }
        public async Task<GetTransactionDto> GetByIdAsync(int id, User loggedUser)
        {
            
            if (!await IsTransactionSenderAsync(id, loggedUser.Id) || !loggedUser.IsAdmin)
            {
                throw new UnauthorizedOperationException(Constants.ModifyAuthorizedErrorMessage);
            }
            var transaction = await this.transactionRepository.GetByIdAsync(id);
            var transactionDto = this.mapper.Map<GetTransactionDto>(transaction);

            return transactionDto;
        }

        public async Task<PaginatedList<Transaction>> FilterByAsync(
            TransactionQueryParameters filterParameters, 
            User loggedUser
            )
        {
            return await this.transactionRepository.FilterByAsync(filterParameters, loggedUser.Username);
        }

        public async Task<Response<GetTransactionDto>> CreateAsync(CreateTransactionDto transactionDto, string loggedUsername)
        {
            var result = new Response<GetTransactionDto>();
            var loggedUser = await this.userRepository.GetByUsernameAsync(loggedUsername);
            var transaction = await MapDtoТоTransactionAsync(transactionDto, loggedUser);

            if (loggedUser.IsBlocked)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionBlockedErrorMessage;
                return result;

               // throw new UnauthorizedOperationException(Constants.ModifyTransactionBlockedErrorMessage);
            }
            
            if (!await this.accountRepository.HasEnoughBalanceAsync(transaction.AccountSenderId, transaction.Amount))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransactionAmountErrorMessage;
                return result;
                // throw new EntityNotFoundException(Constants.ModifyTransactionAmountErrorMessage);
            }

            var newTransaction = await this.transactionRepository.CreateOutTransactionAsync(transaction);
            var getTranactionDto = this.mapper.Map<GetTransactionDto>(newTransaction);
            result.Data = getTranactionDto;

            return result;
        }

        public async Task<Transaction> UpdateAsync(int id, User loggedUser, CreateTransactionDto transactionDto)
        {
            if (!await IsTransactionSenderAsync(id, loggedUser.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAuthorizedErrorMessage);
            }

            var transactionToUpdate = await this.transactionRepository.GetByIdAsync(id);

            if (transactionToUpdate.IsExecuted
                || transactionToUpdate.Direction == DirectionType.In
                || transactionToUpdate.IsDeleted)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionExecuteErrorMessage);
            }

            var newTransaction = await MapDtoТоTransactionAsync(transactionDto, loggedUser);
            return await this.transactionRepository.UpdateAsync(transactionToUpdate, newTransaction);
        }

        public async Task<bool> DeleteAsync(int id, User loggedUser)
        {
            if (!await IsTransactionSenderAsync(id, loggedUser.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAuthorizedErrorMessage);
            }

            var trasaction = await this.transactionRepository.GetByIdAsync(id);
            if (trasaction.IsExecuted || trasaction.Direction == DirectionType.In)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionExecuteErrorMessage); 
            }

            return await this.transactionRepository.DeleteAsync(trasaction);
        }


        public async Task<bool> ExecuteAsync(int transactionId, User loggedUser)
        {
            if (!await IsTransactionSenderAsync(transactionId, loggedUser.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAuthorizedErrorMessage);
            }

            var transactionOut = await this.transactionRepository.GetByIdAsync(transactionId);
            if (transactionOut.IsExecuted
                || transactionOut.Direction == DirectionType.In
                || transactionOut.IsDeleted)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionExecuteErrorMessage); 
            }

            transactionOut.IsExecuted = true;
            transactionOut.Date = DateTime.Now;
            var transactionInAmount = await GetTransactionInAmountAsync(transactionOut);
            var transactionIn = await this.transactionRepository.CreateInTransactionAsync(transactionOut, transactionInAmount);
            
            await this.accountRepository.DecreaseBalanceAsync(transactionOut.AccountSenderId, transactionOut.Amount);
            await this.accountRepository.IncreaseBalanceAsync(transactionIn.AccountRecepientId, transactionIn.Amount);

            await AddTransactionToHistoryAsync(transactionOut);
            await AddTransactionToHistoryAsync(transactionIn);

            return transactionOut.IsExecuted;
        }
        private async Task<decimal> GetTransactionInAmountAsync(Transaction transactionOut)
        {
            var transactionInAmount = transactionOut.Amount;
            var transactionCurrencyCode = transactionOut.Currency.CurrencyCode;
            var recepientAccountCurrencyCode = transactionOut.AccountRecepient.Currency.CurrencyCode;
            
            if (transactionCurrencyCode!= recepientAccountCurrencyCode)
            {
                var exchangeRateDataResult = await exchangeRateService.GetExchangeRateDataAsync(transactionCurrencyCode, recepientAccountCurrencyCode);
                if (exchangeRateDataResult.IsSuccessful)
                {
                    transactionInAmount *= exchangeRateDataResult.Data.CurrencyValue;
                }
            }

            return transactionInAmount;
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
            transaction.AccountRecepient = await this.accountRepository.GetByUsernameAsync(transactionDto.RecepientUsername);
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

    }
}
