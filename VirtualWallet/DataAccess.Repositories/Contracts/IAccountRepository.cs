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

        Account GetById(int id);

        Account GetByUserId(int id);

        Account Create(Account account, User user);

        Account Update(int id, Account account);

        Account Delete(int id);

        Account DepositToBalance(Account account, int amount);

        Account WithdrawalFromBalance(Account account, int amount);


    }
}
