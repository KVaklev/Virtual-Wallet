using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransferRepository
    {
        List<Transfer> GetAll();
        //paginated list

        Transfer GetById(int id);

        Transfer GetByUserId(int userId);

        Transfer Create(Transfer transfer);

        Transfer Update(int id, Transfer transfer);

        Transfer Delete(int id);



        




    }
}
