using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransferRepository
    {
        IQueryable<Transfer> GetAll(User user);
        Task<PaginatedList<Transfer>> FilterByAsync(TransferQueryParameters filterParameters, User user);

        Task<Transfer> GetByIdAsync(int id);

        Task<Transfer> GetByUserIdAsync(int userId);

        Task<Transfer> CreateAsync(Transfer transfer);

        Task<Transfer> UpdateAsync(Transfer transfer);

        Task<bool> DeleteAsync(int id);

        Task<bool> SaveChangesAsync();









    }
}
