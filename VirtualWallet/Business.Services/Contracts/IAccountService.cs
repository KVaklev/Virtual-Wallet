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
        PaginatedList<Account> FilterBy(AccountQueryParameters accountQueryParameters);
        Account GetById(int id);
        Account GetByUsername(string username);
        Account Create(Account account, User user);
        bool AddCard(int id, Card card);
        bool RemoveCard(int id,  Card card);
        bool Delete(int id, User user);

       // bool AccountExists(int id);



    }
}
