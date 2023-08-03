using Business.QueryParameters;
using DataAccess.Models.Models;
using Transaction = DataAccess.Models.Models.Transaction;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(int id);
        Task<PaginatedList<Transaction>> FilterByAsync(TransactionQueryParameters filterParameters, string username);
        Task<bool> SaveChangesAsync();
        Task<bool> DeleteAsync(Transaction transaction);
        Task<Transaction> CreateTransactionAsync(Transaction trasaction);
    }
}
