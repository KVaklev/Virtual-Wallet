using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransferRepository
    {
        IQueryable <Transfer> GetAll(string username);
        PaginatedList<Transfer> FilterBy(TransferQueryParameters filterParameters, string username);
       
        Task <Transfer> GetByIdAsync(int id);

        Task <Transfer> GetByUserIdAsync(int userId);

        Task <Transfer> CreateAsync(Transfer transfer);

        Task <Transfer> UpdateAsync(int id, Transfer transfer);

        Task <bool> DeleteAsync(int id);









    }
}
