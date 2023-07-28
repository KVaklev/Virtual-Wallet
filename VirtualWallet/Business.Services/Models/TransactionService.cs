using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IHistoryRepository historyRepository;
        private readonly ApplicationContext context;
        private readonly IAccountRepository accountRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IHistoryRepository historyRepository,
            ApplicationContext context,
            IAccountRepository accountRepository
            )
        {
            this.transactionRepository = transactionRepository;
            this.historyRepository = historyRepository;
            this.context = context;
            this.accountRepository = accountRepository;
        }

        public Transaction Create(Transaction transaction, User user)
        {
            if (user.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            if (!this.accountRepository.CheckBalance(transaction.AccountSenderId, transaction.Amount))
            {
                throw new EntityNotFoundException(Constants.ModifyTransactionAmountErrorMessage);
            }
            transaction.Direction = DirectionType.Out;
            var newTransaction = this.transactionRepository.CreateOutTransaction(transaction);

            return newTransaction;
        }

        public Transaction GetById(int id, User user)
        {
            if (!IsUserUnauthorized(id, user.Id) || user.IsAdmin != true)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }

            return this.transactionRepository.GetById(id);
        }

        public Transaction Update(int id, User user, Transaction transaction)
        {
            if (!IsUserUnauthorized(id, user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            
            return this.transactionRepository.Update(id, transaction);
        }

        public bool Delete(int id, User user)
        {
            if (!IsUserUnauthorized(id,user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            
            return this.transactionRepository.Delete(id);
        }

        public IQueryable<Transaction> GetAll(string username)
        {
            return this.transactionRepository.GetAll(username);
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
            if (transactionOut.IsExecuted || transactionOut.Direction==DirectionType.In || transactionOut.IsDeleted)
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
            var history = new History();
            history.EventTime = DateTime.Now;
            history.TransactionId = transaction.Id;
            history.NameOperation = NameOperation.Transaction;
           
            if (transaction.Direction==DirectionType.Out)
            {
                history.AccountId = transaction.AccountSenderId;
            }
            else
            {
                history.AccountId = transaction.AccountRecepientId;
            }
            int historyCount = this.context.History.Count();
            this.historyRepository.Create(history);
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
