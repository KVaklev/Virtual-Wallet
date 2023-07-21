using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Transaction Create(Transaction trasaction);

        Transaction GetById(int id);

        Transaction Update(int id, Transaction transaction);

        Transaction Delete(int id);

        List<Transaction> GetAll();
    }
}
