using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transaction = DataAccess.Models.Models.Transaction;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Transaction CreateOutTransaction(Transaction trasaction);

        Transaction CreateInTransaction(Transaction transactionOut);
        Transaction GetById(int id);

        Transaction Update(int id, Transaction transaction);

        bool Delete(int id);

        IQueryable<Transaction> GetAll(string username);

        PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters, string username);

    }
}
