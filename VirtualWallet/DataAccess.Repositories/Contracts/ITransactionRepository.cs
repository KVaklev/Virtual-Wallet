using Business.QueryParameters;
using DataAccess.Models.Models;
using Transaction = DataAccess.Models.Models.Transaction;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        IQueryable<Transaction> GetAll(string username);
        Task<Transaction> GetByIdAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<bool> DeleteAsync(Transaction transaction);
        Task<Transaction> CreateTransactionAsync(Transaction trasaction);
    }
}
