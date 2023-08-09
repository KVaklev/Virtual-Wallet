using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IHistoryRepository
    {
        Task<History> GetByIdAsync(int id);
        Task<PaginatedList<History>> FilterByAsync(HistoryQueryParameters filterParameters, User loggedUser);
        Task<History> CreateAsync(History history);
        Task<int> GetHistoryCountAsync();
    }
}
