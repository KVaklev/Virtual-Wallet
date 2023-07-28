using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IHistoryRepository
    {
        public History CreateWithTransaction(Transaction transaction);

        public History CreateWithTransfer(Transfer transfer);

        History GetById(int id);

        IQueryable<History> GetAll(User user);

        PaginatedList<History> FilterBy(HistoryQueryParameters filterParameters, User user);

        IQueryable<History> Paginate(IQueryable<History> result, int pageNumber, int pageSize);

    }
}
