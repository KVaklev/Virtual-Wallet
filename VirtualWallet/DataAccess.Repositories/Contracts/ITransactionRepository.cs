﻿using Business.QueryParameters;
using DataAccess.Models.Models;
using Transaction = DataAccess.Models.Models.Transaction;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(int id);
        IQueryable<Transaction> GetAll(string username);
        Task<PaginatedList<Transaction>> FilterByAsync(TransactionQueryParameters filterParameters, string username);
        Task<Transaction> UpdateAsync(Transaction transactionToUpdate, Transaction transaction);
        Task<bool> DeleteAsync(Transaction transaction);
        Task<Transaction> CreateInTransactionAsync(Transaction transactionOut);
        Task<Transaction> CreateOutTransactionAsync(Transaction trasaction);

    }
}
