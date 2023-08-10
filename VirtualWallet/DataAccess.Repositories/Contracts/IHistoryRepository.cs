using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IHistoryRepository
    {
        IQueryable<History> GetAll();
        Task<History> GetByIdAsync(int id);
        Task<History> CreateAsync(History history);
        Task<int> GetHistoryCountAsync();
    }
}
