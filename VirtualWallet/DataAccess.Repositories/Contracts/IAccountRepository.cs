using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Contracts
{
    public interface IAccountRepository
    {
        List<Account> GetAll();

        PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters);

        Account GetById(int id);

        Account GetByUserId(int id);

        Account GetByUsername(string username);

        Account Create(Account account, User user);

        Account Update(int id, Account account);

        bool Delete(int id);

        Account IncreaseBalance(int id, int amount);

        Account DecreaseBalance(int id, int amount);

        bool CheckBalance(int id, int amount);


    }
}
