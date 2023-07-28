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

        public TransactionService(
            ITransactionRepository transactionRepository,
            IHistoryRepository historyRepository,
            ApplicationContext context,
            IAccountRepository accountRepository,
            ICurrencyRepository currencyRepository,
            IMapper mapper
            )
        {
            this.transactionRepository = transactionRepository;
            this.historyRepository = historyRepository;
            this.context = context;
            this.accountRepository = accountRepository;
            this.currencyRepository = currencyRepository;
            this.mapper = mapper;
        }

        public Transaction Create(CreateTransactionDto transactionDto, User loggedUser)
        {
            var transaction = MapDtoТоTransaction(transactionDto, loggedUser);

            if (loggedUser.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            if (!this.accountRepository.CheckBalance(transaction.AccountSenderId, transaction.Amount))
            {
                throw new EntityNotFoundException(Constants.ModifyTransactionAmountErrorMessage);
            }
            
            var newTransaction = this.transactionRepository.CreateOutTransaction(transaction);

            return newTransaction;
        }

        public GetTransactionDto GetById(int id, User user)
        {
            if (!IsUserUnauthorized(id, user.Id) || user.IsAdmin != true)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            var transaction = this.transactionRepository.GetById(id);
            var transactionDto = this.mapper.Map<GetTransactionDto>(transaction);

            return transactionDto;
        }

        public Transaction Update(int id, User user, CreateTransactionDto transactionDto)
        {
            if (!IsUserUnauthorized(id, user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }

            var transactionToUpdate = this.transactionRepository.GetById(id);
            
            if (transactionToUpdate.IsExecuted
                || transactionToUpdate.Direction == DirectionType.In
                || transactionToUpdate.IsDeleted)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionUpdateErrorMessage);
            }

            var newTransaction = MapDtoТоTransaction(transactionDto, user);
            return this.transactionRepository.Update(transactionToUpdate, newTransaction);
        }

        public bool Delete(int id, User user)
        {
            if (!IsUserUnauthorized(id,user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }

            var trasaction = this.transactionRepository.GetById(id);
            if (trasaction.IsExecuted || trasaction.Direction == DirectionType.In)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionUpdateErrorMessage);
            }

            return this.transactionRepository.Delete(trasaction);
        }

        public PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters, User user)
        {
            var result = this.transactionRepository.FilterBy(filterParameters, user.Username);
            if (result.Count==0)
            {
                throw new EntityNotFoundException(Constants.ModifyTransactionNoDataErrorMessage);
            }
            return result;
        }

        public bool Execute(int transactionId, User user)
        {
            if (!IsUserUnauthorized(transactionId,user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            
            var transactionOut = this.transactionRepository.GetById(transactionId);
            if (transactionOut.IsExecuted 
                || transactionOut.Direction==DirectionType.In 
                || transactionOut.IsDeleted)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }

            transactionOut.IsExecuted = true;
            transactionOut.Date = DateTime.Now;

            var transactionIn = this.transactionRepository.CreateInTransaction(transactionOut);

            this.accountRepository.DecreaseBalance(transactionOut.AccountSenderId, transactionOut.Amount);
            this.accountRepository.IncreaseBalance(transactionIn.AccountRecepientId, transactionIn.Amount);

            AddTransactionToHistory(transactionOut);
            AddTransactionToHistory(transactionIn);

            return transactionOut.IsExecuted;
        }
        
        private bool AddTransactionToHistory(Transaction transaction)
        {
            
            int historyCount = this.context.History.Count();
            this.historyRepository.CreateWithTransaction(transaction);
            int newHistoryCount = this.context.History.Count();

            if (newHistoryCount == historyCount + 1)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private Transaction MapDtoТоTransaction(CreateTransactionDto transactionDto, User user)
        {
            var transaction = this.mapper.Map<Transaction>(transactionDto);
            transaction.AccountSenderId = (int)user.AccountId;
            transaction.AccountSender = user.Account;
            transaction.AccountRecepientId = this.accountRepository
                                                 .GetByUsername(transactionDto.RecepientUsername)
                                                 .Id;
            transaction.CurrencyId = this.currencyRepository
                                         .GetByАbbreviation(transactionDto.Abbreviation)
                                         .Id;
            transaction.Direction = DirectionType.Out;
            return transaction;
        }

        private bool IsUserUnauthorized(int id, int userId)
        {
            bool isUserUnauthorized = true;
            
            Transaction transactionToUpdate = this.transactionRepository.GetById(id);

            if (transactionToUpdate.AccountSender.User.Id != userId)
            {
                isUserUnauthorized = false;
            }
            return isUserUnauthorized;
        }

    }
}
