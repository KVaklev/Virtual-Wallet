using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Contracts
{
    public interface ITransactionService
    {
        Transaction Create(Transaction transaction, User user);
        Transaction GetById(int id, User user);
        Transaction Update(int id, int userId, Transaction transaction);
        bool Delete(int id, int userId);
        IQueryable<Transaction> GetAll();
        PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters, User user);
        public bool Execute(int transactionId, int userId);
    }
}
