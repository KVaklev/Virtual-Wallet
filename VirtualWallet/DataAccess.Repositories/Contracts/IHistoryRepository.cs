using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IHistoryRepository
    {
        IQueryable<History> GetAll(User loggedUser);
        Task<History> GetByIdAsync(int id);
        Task<PaginatedList<History>> FilterByAsync(HistoryQueryParameters filterParameters, User loggedUser);
        Task<History> CreateWithTransactionAsync(Transaction transaction);
        Task<History> CreateWithTransferAsync(Transfer transfer);

    }
}
