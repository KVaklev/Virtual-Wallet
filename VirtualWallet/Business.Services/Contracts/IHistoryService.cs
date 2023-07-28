using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IHistoryService
    {
        History GetById(int id, User user);

        IQueryable<History> GetAll(User user);

        PaginatedList<History> FilterBy(HistoryQueryParameters filterParameters, User user);
    }
}
