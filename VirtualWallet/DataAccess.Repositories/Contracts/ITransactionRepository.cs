using DataAccess.Models.Contracts;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        ITransaction Create(Transaction trasaction);

        ITransaction GetById(int id);

        ITransaction Update(int id, Transaction transaction);

        ITransaction Delete(int id);

        List<ITransaction> GetAll();
    }
}
