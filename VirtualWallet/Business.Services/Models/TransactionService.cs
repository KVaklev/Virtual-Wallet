using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Models
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionRepository transactionRepository;
        private readonly HistoryRepository historyRepository;
        private readonly UserRepository userRepository;
        private readonly ApplicationContext context;

        public TransactionService(
            TransactionRepository transactionRepository,
            HistoryRepository historyRepository,
            UserRepository userRepository,
            ApplicationContext context
            )
        {
            this.transactionRepository = transactionRepository;
            this.historyRepository = historyRepository;
            this.userRepository = userRepository;
            this.context = context;
        }

        public Transaction Create(Transaction transaction, User user)
        {
            if (user.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }
            var newTransaction = this.transactionRepository.Create(transaction);

            //Todo - check the balance
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

        public IQueryable<Transaction> GetAll()
        {
            //Todo - add authorized user
            return this.transactionRepository.GetAll();
        }

        public PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters, User user)
        {
            //Todo - add authorized user
            return this.transactionRepository.FilterBy(filterParameters);
        }

        public bool Execute(int transactionId, int userId)
        {
            if (!IsUserUnauthorized(transactionId,userId))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }
            var transaction = this.transactionRepository.GetById(transactionId);
            transaction.IsExecuted = true;

            //Todo - change balance.
            AddTransactionToHistory(transaction);

            return transaction.IsExecuted;
        }
        
        private bool AddTransactionToHistory(Transaction transaction)
        {
            var history = new History()
            {
                EventTime = transaction.Date,
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

            if (transactionToUpdate.SenderId != userId)
            {
                isUserUnauthorized = false;
            }
            return isUserUnauthorized;
        }
    }
}
