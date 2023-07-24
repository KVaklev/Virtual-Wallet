using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Contracts
{
    public interface IAccountService
    {
        IQueryable<Account> GetAll();
        PaginatedList<Account> FilterBy(AccountQueryParameters accaountQueryParameters);
        Account GetById(int id);
        Account GetByUsername(string username);
        Account Create(Account account);
        Account Update(int id, Account account);
        bool Delete(int id);



    }
}
