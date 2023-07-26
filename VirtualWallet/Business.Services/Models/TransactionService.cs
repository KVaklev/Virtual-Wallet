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
        private readonly IUserRepository userRepository;
        private readonly ApplicationContext context;
        private readonly IAccountRepository accountRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IHistoryRepository historyRepository,
            IUserRepository userRepository,
            ApplicationContext context
            )
        {
            this.transactionRepository = transactionRepository;
            this.historyRepository = historyRepository;
            this.userRepository = userRepository;
            this.context = context;
            this.accountRepository = accountRepository;
        }

        public Transaction Create(Transaction transaction, User user)
        {
            if (user.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }
            //if (this.accountRepository.CheckBalance(transaction.AccountSenderId, transaction.Amount))
            //{
            //    throw new EntityNotFoundException(Constants.ModifyTransactionAmountErrorMessage);
            //}
            transaction.Direction = DirectionType.Out;
            var newTransaction = this.transactionRepository.CreateOutTransaction(transaction);

            return newTransaction;
        }

        public Transaction GetById(int id, User user)
        {
            if (!IsUserUnauthorized(id, user.Id) || user.IsAdmin != true)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }

            return this.transactionRepository.GetById(id);
        }

        public Transaction Update(int id, int userId, Transaction transaction)
        {
            if (!IsUserUnauthorized(id, userId))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }
            return this.transactionRepository.Update(id, transaction);
        }

        public bool Delete(int id, int userId)
        {
            if (!IsUserUnauthorized(id,userId))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }
            return this.transactionRepository.Delete(id);
        }

        public IQueryable<Transaction> GetAll(string username)
        {
            return this.transactionRepository.GetAll(username);
        }

        public PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters, User user)
        {
            return this.transactionRepository.FilterBy(filterParameters, user.Username);
        }

        public bool Execute(int transactionId, int userId)
        {
            if (!IsUserUnauthorized(transactionId,userId))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }
            var transactionOut = this.transactionRepository.GetById(transactionId);
            transactionOut.IsExecuted = true;

            var transactionIn = this.transactionRepository.CreateInTransaction(transactionOut);

           // this.accountRepository.DecreaseBalance(transactionOut.AccountSenderId, transactionOut.Amount);
           // this.accountRepository.IncreaseBalance(transactionIn.AccountRecepientId, transactionIn.Amount);

            AddTransactionToHistory(transactionOut);
            AddTransactionToHistory(transactionIn);

            return transactionOut.IsExecuted;
        }
        
        private bool AddTransactionToHistory(Transaction transaction)
        {
            var history = new History()
            {
                EventTime = DateTime.Now,
                TransactionId = transaction.Id
            };

            int historyCount = this.context.History.Count();
            this.historyRepository.Ctraete(history);
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
