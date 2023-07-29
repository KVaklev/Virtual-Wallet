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
        Task<IQueryable<Account>> GetAll();

        PaginatedList<Account> FilterBy(AccountQueryParameters filterParameters);

        Account GetById(int id);

        Task<Account> GetByUsernameAsync(string username);

        Task<Account> Create(Account account, User user);

        bool AddCard(int id, Card card);

        bool RemoveCard(int id, Card card);

        bool Delete(int id);

        Account IncreaseBalance(int id, decimal amount);

        Account DecreaseBalance(int id, decimal amount);

        Task<bool> HasEnoughBalanceAsync(int id, decimal amount);

        public bool CardExists(string cardNumber);

        public bool AccountExists(int id);

        // bool AccountExists(int id);


    }
}
